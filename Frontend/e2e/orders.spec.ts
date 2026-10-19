import { expect, test } from "@playwright/test";
import axe from "axe-core";
test("login, crear, listar, editar y eliminar; accesibilidad y móvil", async ({
  page,
}) => {
  const orders: Record<string, unknown>[] = [];
  await page.route("https://localhost:7027/api/v1/**", async (route) => {
    const path = new URL(route.request().url()).pathname;
    const method = route.request().method();
    let data: unknown = [];
    if (path.endsWith("/login"))
      data = {
        token: `header.${Buffer.from(JSON.stringify({ name: "Ana", role: "Admin", exp: Math.floor(Date.now() / 1000) + 300 })).toString("base64url")}.signature`,
      };
    if (path.endsWith("/clientes/"))
      data = [{ clienteId: 1, nombre: "Ana", apellido: "Pérez" }];
    if (path.endsWith("/vehiculos/"))
      data = [{ vehiculoId: 1, clienteId: 1, patente: "ABCD12" }];
    if (path.endsWith("/mecanicos/"))
      data = [{ mecanicoId: 1, nombre: "Luis", apellido: "Soto" }];
    if (path.includes("/ordenes-trabajo")) {
      if (method === "POST")
        orders.push({ ...route.request().postDataJSON(), ordenId: 1 });
      if (method === "PUT")
        orders[0] = { ...route.request().postDataJSON(), ordenId: 1 };
      if (method === "DELETE") orders.splice(0);
      data = /\/1$/.test(path) ? orders[0] : orders;
    }
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(data ?? null),
    });
  });
  await page.goto("/ordenes");
  await page.getByLabel("Usuario", { exact: true }).fill("ana");
  await page.getByLabel("Contraseña", { exact: true }).fill("secreto");
  await page.getByRole("button", { name: /Entrar al taller/ }).click();
  await page.getByRole("link", { name: "+ Nueva orden" }).click();
  await page.getByLabel("Cliente", { exact: true }).selectOption("1");
  await page.getByLabel("Vehículo del cliente").selectOption("1");
  await page.getByLabel("Mecánico asignado").selectOption("1");
  await page
    .getByLabel("Observaciones (opcional)")
    .fill("<script>alert(1)</script>");
  await page.getByRole("button", { name: "Guardar orden" }).click();
  await expect(
    page.getByText("<script>alert(1)</script>", { exact: true }),
  ).toBeVisible();
  await page.evaluate(axe.source);
  const result = await page.evaluate(async () => {
    const a = (
      window as unknown as {
        axe: { run: (options: unknown) => Promise<{ violations: unknown[] }> };
      }
    ).axe;
    return a.run({
      runOnly: {
        type: "tag",
        values: ["wcag2a", "wcag2aa", "wcag21aa", "wcag22aa"],
      },
    });
  });
  expect(result.violations).toEqual([]);
  await page.setViewportSize({ width: 320, height: 740 });
  expect(
    await page.evaluate(
      () => document.documentElement.scrollWidth <= window.innerWidth,
    ),
  ).toBe(true);
  await page.getByRole("link", { name: "Ver orden 1" }).click();
  await page.getByRole("link", { name: "Editar orden" }).click();
  await page.getByLabel("Estado", { exact: true }).selectOption("Completada");
  await page.getByRole("button", { name: "Guardar orden" }).click();
  await expect(page.getByText("Completada", { exact: true })).toBeVisible();
  await page
    .getByRole("button", { name: "Eliminar orden", exact: true })
    .click();
  await expect(page.getByRole("dialog")).toBeVisible();
  await expect(
    page.getByRole("button", { name: "Cancelar", exact: true }),
  ).toBeFocused();
  await page
    .getByRole("dialog")
    .getByRole("button", { name: "Eliminar orden", exact: true })
    .click();
  await expect(
    page.getByText("Tu próxima reparación empieza aquí"),
  ).toBeVisible();
});
