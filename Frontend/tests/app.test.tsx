import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { MemoryRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AuthProvider } from "../src/auth";
import { setToken } from "../src/api";
import App from "../src/App";
const token = (role = "Admin") =>
  `header.${btoa(JSON.stringify({ name: "Ana", role, exp: Math.floor(Date.now() / 1000) + 300 }))}.signature`;
function mount(path = "/login") {
  return render(
    <QueryClientProvider
      client={
        new QueryClient({
          defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
          },
        })
      }
    >
      <AuthProvider>
        <MemoryRouter initialEntries={[path]}>
          <App />
        </MemoryRouter>
      </AuthProvider>
    </QueryClientProvider>,
  );
}
function mockApi(role = "Admin") {
  return vi
    .spyOn(globalThis, "fetch")
    .mockImplementation(async (url, options) => {
      const path = String(url);
      let data: unknown = [];
      if (path.endsWith("/login")) data = { token: token(role) };
      if (path.endsWith("/clientes/"))
        data = [{ clienteId: 1, nombre: "Ana", apellido: "Pérez" }];
      if (path.endsWith("/vehiculos/"))
        data = [{ vehiculoId: 1, clienteId: 1, patente: "ABCD12" }];
      if (path.endsWith("/mecanicos/"))
        data = [{ mecanicoId: 1, nombre: "Luis", apellido: "Soto" }];
      if (options?.method === "POST" && path.endsWith("/ordenes-trabajo/"))
        data = { ordenId: 1 };
      return new Response(JSON.stringify(data), {
        status: 200,
        headers: { "Content-Type": "application/json" },
      });
    });
}
async function login() {
  const user = userEvent.setup();
  await user.type(screen.getByLabelText("Usuario"), "ana");
  await user.type(screen.getByLabelText("Contraseña"), "secreto");
  await user.click(screen.getByRole("button", { name: /Entrar al taller/ }));
  return user;
}
beforeEach(() => {
  vi.restoreAllMocks();
  setToken(null);
});
it("oculta usuarios y roles al mecánico", async () => {
  mockApi("Mecanico");
  mount();
  const user = await login();
  await user.click(
    await screen.findByRole("link", { name: "Datos del taller" }),
  );
  expect(
    screen.queryByRole("link", { name: "Usuarios", exact: true }),
  ).not.toBeInTheDocument();
  expect(
    screen.queryByRole("link", { name: "Roles", exact: true }),
  ).not.toBeInTheDocument();
});
it("muestra usuarios y roles al administrador", async () => {
  mockApi();
  mount();
  const user = await login();
  await user.click(
    await screen.findByRole("link", { name: "Datos del taller" }),
  );
  expect(
    screen.getByRole("link", { name: "Usuarios", exact: true }),
  ).toBeInTheDocument();
  expect(
    screen.getByRole("link", { name: "Roles", exact: true }),
  ).toBeInTheDocument();
});
it("permite cargar un cliente desde Datos del taller", async () => {
  const fetch = mockApi();
  mount();
  const user = await login();
  await user.click(
    await screen.findByRole("link", { name: "Datos del taller" }),
  );
  await user.click(screen.getByRole("link", { name: "Clientes" }));
  await user.type(await screen.findByLabelText("Nombre"), "Pedro");
  await user.type(screen.getByLabelText("Apellido"), "Soto");
  await user.type(
    screen.getByLabelText("Correo electrónico"),
    "pedro@example.com",
  );
  await user.type(screen.getByLabelText("Teléfono"), "912345678");
  await user.click(screen.getByRole("button", { name: "Guardar registro" }));
  await waitFor(() =>
    expect(fetch).toHaveBeenCalledWith(
      expect.stringContaining("/clientes/"),
      expect.objectContaining({
        method: "POST",
        body: JSON.stringify({
          nombre: "Pedro",
          apellido: "Soto",
          correo: "pedro@example.com",
          telefono: "912345678",
          direccion: "",
        }),
      }),
    ),
  );
  expect(await screen.findByText("Registro creado.")).toBeInTheDocument();
});
it("indica que se debe crear una marca antes de un modelo", async () => {
  mockApi();
  mount();
  const user = await login();
  await user.click(
    await screen.findByRole("link", { name: "Datos del taller" }),
  );
  await user.click(screen.getByRole("link", { name: "Modelos" }));
  expect(await screen.findByText(/Primero registra/)).toBeInTheDocument();
  expect(
    screen.getByRole("button", { name: "Guardar registro" }),
  ).toBeDisabled();
});
it("registra una cuenta y permite iniciar sesión con las mismas credenciales", async () => {
  const fetch = mockApi("Mecanico");
  mount("/registro");
  const user = userEvent.setup();
  await user.type(screen.getByLabelText("Usuario"), "ana");
  await user.type(
    screen.getByLabelText("Correo electrónico"),
    "ana@example.com",
  );
  await user.type(
    screen.getByLabelText("Contraseña (mínimo 6 caracteres)"),
    "secreto",
  );
  await user.type(screen.getByLabelText("Confirmar contraseña"), "secreto");
  await user.click(screen.getByRole("button", { name: "Crear cuenta" }));
  expect(await screen.findByRole("status")).toHaveTextContent("Cuenta creada");
  expect(fetch).toHaveBeenCalledWith(
    expect.stringContaining("/usuarios/registro"),
    expect.objectContaining({
      method: "POST",
      body: JSON.stringify({
        nombre: "ana",
        correo: "ana@example.com",
        password: "secreto",
      }),
    }),
  );
  await user.click(screen.getByRole("link", { name: "Ir a iniciar sesión" }));
  await login();
  expect(
    await screen.findByRole("heading", { name: "Órdenes de trabajo" }),
  ).toBeInTheDocument();
});
describe("Autenticación y órdenes", () => {
  it("redirige una ruta privada al login", () => {
    mount("/ordenes");
    expect(
      screen.getByRole("heading", { name: "Iniciar sesión" }),
    ).toBeInTheDocument();
  });
  it("inicia sesión y adjunta Bearer al cargar las órdenes", async () => {
    const fetch = mockApi();
    mount();
    await login();
    expect(
      await screen.findByRole("heading", { name: "Órdenes de trabajo" }),
    ).toBeInTheDocument();
    await waitFor(() =>
      expect(fetch).toHaveBeenCalledWith(
        expect.stringContaining("/ordenes-trabajo/"),
        expect.objectContaining({
          headers: expect.objectContaining({
            Authorization: expect.stringContaining("Bearer "),
          }),
        }),
      ),
    );
  });
  it("muestra el error de credenciales sin revelar detalles", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue(
      new Response(null, { status: 401 }),
    );
    mount();
    await login();
    expect(await screen.findByRole("alert")).toHaveTextContent(
      "Credenciales incorrectas",
    );
  });
  it("rechaza roles no autorizados", async () => {
    mockApi("Recepcionista");
    mount();
    await login();
    expect(
      await screen.findByRole("heading", { name: "Acceso denegado" }),
    ).toBeInTheDocument();
  });
  it("crea una orden con relaciones válidas y regresa al listado", async () => {
    const fetch = mockApi();
    mount();
    const user = await login();
    await user.click(
      await screen.findByRole("link", { name: "+ Nueva orden" }),
    );
    await user.selectOptions(await screen.findByLabelText("Cliente"), "1");
    await user.selectOptions(
      screen.getByLabelText("Vehículo del cliente"),
      "1",
    );
    await user.selectOptions(screen.getByLabelText("Mecánico asignado"), "1");
    await user.type(
      screen.getByLabelText("Observaciones (opcional)"),
      "Revisar frenos",
    );
    await user.click(screen.getByRole("button", { name: "Guardar orden" }));
    await waitFor(() =>
      expect(fetch).toHaveBeenCalledWith(
        expect.stringContaining("/ordenes-trabajo/"),
        expect.objectContaining({
          method: "POST",
          body: expect.stringContaining("Revisar frenos"),
        }),
      ),
    );
    expect(
      await screen.findByRole("heading", { name: "Órdenes de trabajo" }),
    ).toBeInTheDocument();
  });
  it("asocia los errores de validación al campo correspondiente", async () => {
    mockApi();
    mount();
    const user = await login();
    await user.click(
      await screen.findByRole("link", { name: "+ Nueva orden" }),
    );
    await user.click(
      await screen.findByRole("button", { name: "Guardar orden" }),
    );
    expect(screen.getByLabelText("Cliente")).toHaveAttribute(
      "aria-invalid",
      "true",
    );
    expect(screen.getByRole("alert")).toHaveFocus();
  });
});
