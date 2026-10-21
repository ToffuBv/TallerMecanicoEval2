import { z } from "zod";
import {
  orderSchema,
  clientSchema,
  vehicleSchema,
  mechanicSchema,
  type OrderInput,
} from "./contracts";
let token: string | null = null;
export const setToken = (value: string | null) => {
  token = value;
};
export class ApiError extends Error {
  constructor(
    public status: number,
    message: string,
    public errors: Record<string, string[]> = {},
  ) {
    super(message);
  }
}
const base = (
  import.meta.env.VITE_API_URL || "https://localhost:7027/api/v1"
).replace(/\/$/, "");
export async function request(
  path: string,
  options: RequestInit = {},
): Promise<unknown> {
  let response: Response;
  try {
    response = await fetch(`${base}${path}`, {
      ...options,
      headers: {
        ...(options.body ? { "Content-Type": "application/json" } : {}),
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        ...options.headers,
      },
    });
  } catch {
    throw new ApiError(
      0,
      "No se pudo conectar con la API. Revisa que esté iniciada y que su certificado HTTPS sea de confianza.",
    );
  }
  const data =
    response.status === 204 ? null : await response.json().catch(() => null);
  if (!response.ok) {
    if (response.status === 401 && token)
      window.dispatchEvent(new Event("session-expired"));
    if (response.status === 403)
      window.dispatchEvent(new Event("access-denied"));
    const messages: Record<number, string> = {
      400: "Revisa los datos del formulario.",
      401: "Credenciales incorrectas o sesión expirada.",
      403: "Tu rol no permite esta acción.",
      404: "El registro ya no existe.",
      409: "El registro tiene relaciones que impiden esta operación.",
      422: "Revisa los datos del formulario.",
      429: "Demasiados intentos. Espera un minuto y vuelve a intentar.",
    };
    throw new ApiError(
      response.status,
      (response.status === 409 && typeof data?.message === "string"
        ? data.message
        : messages[response.status]) ||
        "No fue posible completar la operación.",
      data?.errors || {},
    );
  }
  return data;
}
const checked = async <T>(path: string, schema: z.ZodType<T>): Promise<T> => {
  const parsed = schema.safeParse(await request(path));
  if (!parsed.success)
    throw new ApiError(
      0,
      "La respuesta de la API tiene un formato inesperado.",
    );
  return parsed.data;
};
export const api = {
  register: (nombre: string, correo: string, password: string) =>
    request("/usuarios/registro", {
      method: "POST",
      body: JSON.stringify({ nombre, correo, password }),
    }),
  login: (nombreUsuario: string, password: string) =>
    request("/usuarios/login", {
      method: "POST",
      body: JSON.stringify({ nombreUsuario, password }),
    }).then((data) => z.object({ token: z.string() }).parse(data)),
  orders: () => checked("/ordenes-trabajo/", z.array(orderSchema)),
  order: (id: string) => checked(`/ordenes-trabajo/${id}`, orderSchema),
  clients: () => checked("/clientes/", z.array(clientSchema)),
  vehicles: () => checked("/vehiculos/", z.array(vehicleSchema)),
  mechanics: () => checked("/mecanicos/", z.array(mechanicSchema)),
  save: (input: OrderInput, id?: string) =>
    request(`/ordenes-trabajo/${id || ""}`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(input),
    }),
  remove: (id: number) =>
    request(`/ordenes-trabajo/${id}`, { method: "DELETE" }),
};
