import { test, expect } from "@playwright/test";
import { readFileSync } from "node:fs";

test("CRUD real desde las vistas de servicios, repuestos, roles y pagos", async ({
  page,
}) => {
  test.skip(
    process.env.TALLER_LIVE_TEST !== "1",
    "Requiere autorización y API local con SQL Server",
  );
  test.setTimeout(120000);
  // Solo se incorpora el token real a la sesión; todas las operaciones de datos
  // las realiza el navegador contra la API real, sin simular sus respuestas.
  const token = readFileSync(".admin-test-token", "utf8")
    .trim()
    .replace(/^Bearer /, "");
  await page.route("**/usuarios/login", (route) =>
    route.fulfill({ json: { token } }),
  );
  await page.goto("/login");
  await page.getByLabel("Usuario", { exact: true }).fill("sesion-prueba");
  await page
    .getByLabel("Contraseña", { exact: true })
    .fill("token-suministrado");
  await page.getByRole("button", { name: /Entrar al taller/ }).click();
  await expect(
    page.getByRole("heading", { name: "Órdenes de trabajo" }),
  ).toBeVisible();
  const tag = `UIQA${Date.now()}`;
  for (const item of [
    {
      title: "Servicios",
      route: "servicios",
      fields: {
        Nombre: tag + "Servicio",
        Descripción: "Prueba desde formulario",
        Costo: "1234.50",
      },
    },
    {
      title: "Repuestos",
      route: "repuestos",
      fields: {
        Nombre: tag + "Repuesto",
        Descripción: "Prueba desde formulario",
        Precio: "123.45",
        Stock: "0",
      },
    },
    {
      title: "Roles",
      route: "rol",
      fields: {
        Nombre: tag + "Rol",
        Descripción: "Rol sin usuarios de prueba",
      },
    },
  ]) {
    await page
      .getByRole("link", { name: "Datos del taller", exact: true })
      .click();
    await page.getByRole("link", { name: item.title, exact: true }).click();
    for (const [field, value] of Object.entries(item.fields))
      await page.getByLabel(field, { exact: true }).fill(value);
    const created = page.waitForResponse(
      (r) =>
        r.url().endsWith(`/${item.route}/`) && r.request().method() === "POST",
    );
    await page
      .getByRole("button", { name: "Guardar registro", exact: true })
      .click();
    expect((await created).status()).toBe(201);
    await expect(
      page.getByRole("cell", { name: item.fields.Nombre, exact: true }),
    ).toBeVisible();
    await page
      .getByRole("button", {
        name: `Editar ${item.fields.Nombre}`,
        exact: true,
      })
      .click();
    await page
      .getByLabel("Nombre", { exact: true })
      .fill(item.fields.Nombre + " editado");
    await page
      .getByRole("button", { name: "Guardar registro", exact: true })
      .click();
    await expect(
      page.getByRole("cell", {
        name: item.fields.Nombre + " editado",
        exact: true,
      }),
    ).toBeVisible();
    if (process.env.TALLER_KEEP_TEST_DATA === "1") continue;
    await page
      .getByRole("button", {
        name: `Eliminar ${item.fields.Nombre} editado`,
        exact: true,
      })
      .click();
    await page
      .getByRole("button", { name: "Confirmar eliminación", exact: true })
      .click();
    await expect(
      page.getByRole("cell", {
        name: item.fields.Nombre + " editado",
        exact: true,
      }),
    ).toHaveCount(0);
  }
  // Crear una orden propia evita duplicar el pago de una orden existente.
  await page
    .getByRole("link", { name: "Órdenes de trabajo", exact: true })
    .click();
  await page.getByRole("link", { name: "+ Nueva orden" }).click();
  await page.getByLabel("Cliente", { exact: true }).selectOption("1");
  await page
    .getByLabel("Vehículo del cliente", { exact: true })
    .selectOption("1");
  await page.getByLabel("Mecánico asignado", { exact: true }).selectOption("1");
  await page.getByLabel("Observaciones (opcional)").fill(tag + " Orden pago");
  const orderResponse = page.waitForResponse(
    (r) =>
      r.url().endsWith("/ordenes-trabajo/") && r.request().method() === "POST",
  );
  await page
    .getByRole("button", { name: "Guardar orden", exact: true })
    .click();
  const order = await (await orderResponse).json();
  await page
    .getByRole("link", { name: "Datos del taller", exact: true })
    .click();
  await page.getByRole("link", { name: "Pagos", exact: true }).click();
  await page
    .getByLabel("Orden de trabajo", { exact: true })
    .selectOption(String(order.ordenId));
  await page.getByLabel("Monto total", { exact: true }).fill("2345.67");
  await page.getByLabel("Método de pago", { exact: true }).fill(tag);
  await page
    .getByLabel("Fecha de pago (opcional)", { exact: true })
    .fill("2026-10-20T12:00");
  await page.getByLabel("Estado del pago", { exact: true }).fill("Pendiente");
  const paymentResponse = page.waitForResponse(
    (r) => r.url().endsWith("/pago/") && r.request().method() === "POST",
  );
  await page
    .getByRole("button", { name: "Guardar registro", exact: true })
    .click();
  const payment = await (await paymentResponse).json();
  await expect(
    page.getByRole("cell", { name: tag, exact: true }),
  ).toBeVisible();
  await page
    .getByRole("button", {
      name: `Editar Pago #${payment.pagoId}`,
      exact: true,
    })
    .click();
  await page.getByLabel("Monto total", { exact: true }).fill("3456.78");
  await page
    .getByRole("button", { name: "Guardar registro", exact: true })
    .click();
  await expect(
    page.getByRole("cell", { name: "3456.78", exact: true }),
  ).toBeVisible();
  if (process.env.TALLER_KEEP_TEST_DATA === "1") return;
  await page
    .getByRole("button", {
      name: `Eliminar Pago #${payment.pagoId}`,
      exact: true,
    })
    .click();
  await page
    .getByRole("button", { name: "Confirmar eliminación", exact: true })
    .click();
  await expect(page.getByRole("cell", { name: tag, exact: true })).toHaveCount(
    0,
  );
});
