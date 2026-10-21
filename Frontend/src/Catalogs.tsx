import { useState, type FormEvent } from "react";
import { Link, Navigate, useParams } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { z } from "zod";
import { request, ApiError } from "./api";
import { useAuth } from "./auth";

type Row = Record<string, string | number | null>;
type Field = {
  name: string;
  label: string;
  type?: "email" | "tel" | "number" | "datetime-local" | "password";
  min?: number;
  step?: number;
  max?: number;
  optional?: boolean;
  relation?: string;
};
type Catalog = { title: string; endpoint: string; id: string; fields: Field[] };
const name: Field = { name: "nombre", label: "Nombre", max: 100 };
const surname: Field = { name: "apellido", label: "Apellido", max: 100 };
const phone: Field = {
  name: "telefono",
  label: "Teléfono",
  type: "tel",
  max: 20,
};
const catalogs: Record<string, Catalog> = {
  clientes: {
    title: "Clientes",
    endpoint: "clientes",
    id: "clienteId",
    fields: [
      name,
      surname,
      { name: "correo", label: "Correo electrónico", type: "email", max: 150 },
      phone,
      { name: "direccion", label: "Dirección", max: 255, optional: true },
    ],
  },
  marcas: { title: "Marcas", endpoint: "marca", id: "marcaId", fields: [name] },
  modelos: {
    title: "Modelos",
    endpoint: "modelo",
    id: "modeloId",
    fields: [name, { name: "marcaId", label: "Marca", relation: "marcas" }],
  },
  vehiculos: {
    title: "Vehículos",
    endpoint: "vehiculos",
    id: "vehiculoId",
    fields: [
      { name: "patente", label: "Patente", max: 20 },
      { name: "anio", label: "Año", type: "number", optional: true },
      { name: "color", label: "Color", max: 50 },
      { name: "clienteId", label: "Cliente", relation: "clientes" },
      { name: "modeloId", label: "Modelo", relation: "modelos" },
    ],
  },
  especialidades: {
    title: "Especialidades",
    endpoint: "especialidad",
    id: "especialidadId",
    fields: [name, { name: "descripcion", label: "Descripción", max: 255 }],
  },
  mecanicos: {
    title: "Mecánicos",
    endpoint: "mecanicos",
    id: "mecanicoId",
    fields: [
      name,
      surname,
      phone,
      {
        name: "especialidadId",
        label: "Especialidad",
        relation: "especialidades",
      },
    ],
  },
};
Object.assign(catalogs, {
  servicios: {
    title: "Servicios",
    endpoint: "servicios",
    id: "servicioId",
    fields: [
      name,
      { name: "descripcion", label: "Descripción", max: 255 },
      { name: "costo", label: "Costo", type: "number", min: 0.01, step: 0.01 },
    ],
  },
  repuestos: {
    title: "Repuestos",
    endpoint: "repuestos",
    id: "repuestoId",
    fields: [
      name,
      { name: "descripcion", label: "Descripción", max: 255 },
      {
        name: "precio",
        label: "Precio",
        type: "number",
        min: 0.01,
        step: 0.01,
      },
      { name: "stock", label: "Stock", type: "number", min: 0 },
    ],
  },
  pagos: {
    title: "Pagos",
    endpoint: "pago",
    id: "pagoId",
    fields: [
      { name: "ordenId", label: "Orden de trabajo", relation: "ordenes" },
      {
        name: "montoTotal",
        label: "Monto total",
        type: "number",
        min: 0.01,
        step: 0.01,
      },
      { name: "metodoPago", label: "Método de pago", max: 50 },
      {
        name: "fechaPago",
        label: "Fecha de pago",
        type: "datetime-local",
        optional: true,
      },
      { name: "estado", label: "Estado del pago", max: 50 },
    ],
  },
  roles: {
    title: "Roles",
    endpoint: "rol",
    id: "rolId",
    fields: [
      { ...name, max: 50 },
      { name: "descripcion", label: "Descripción", max: 255 },
    ],
  },
  usuarios: {
    title: "Usuarios",
    endpoint: "usuarios",
    id: "usuarioId",
    fields: [
      name,
      { name: "correo", label: "Correo electrónico", type: "email", max: 150 },
      { name: "rolId", label: "Rol", relation: "roles" },
      { name: "password", label: "Contraseña", type: "password", max: 200 },
    ],
  },
} satisfies Record<string, Catalog>);
const relations: Record<string, Catalog> = {
  ...catalogs,
  ordenes: {
    title: "Órdenes de trabajo",
    endpoint: "ordenes-trabajo",
    id: "ordenId",
    fields: [],
  },
};
const rowSchema = z.object({}).catchall(z.unknown());
async function load(config: Catalog): Promise<Row[]> {
  const rows = z.array(rowSchema).parse(await request(`/${config.endpoint}/`));
  return rows.map(
    (row) =>
      Object.fromEntries(
        Object.entries(row).filter(
          ([, value]) =>
            value === null ||
            typeof value === "string" ||
            typeof value === "number",
        ),
      ) as Row,
  );
}
function label(row: Row) {
  if (row.pagoId != null) return `Pago #${row.pagoId}`;
  if (row.ordenId != null)
    return `Orden #${row.ordenId} · ${row.estado || "Sin estado"}`;
  return [row.nombre || row.patente, row.apellido].filter(Boolean).join(" ");
}
function Errors({ error }: { error: unknown }) {
  if (!error) return null;
  return (
    <div className="notice error" role="alert">
      {error instanceof Error
        ? error.message
        : "No se pudo completar la operación."}
      {error instanceof ApiError &&
        Object.entries(error.errors).map(([key, messages]) => (
          <p key={key}>{messages.join(" ")}</p>
        ))}
    </div>
  );
}
export default function Catalogs() {
  const { session } = useAuth();
  const { catalog } = useParams();
  const config = catalog ? catalogs[catalog] : undefined;
  const adminOnly = (key: string) => ["usuarios", "roles"].includes(key);
  if (catalog && adminOnly(catalog) && session?.role !== "Admin")
    return <Navigate to="/acceso-denegado" replace />;
  return (
    <>
      <h1 tabIndex={-1}>Datos del taller</h1>
      <p className="muted">
        Carga clientes, marcas y especialidades; después modelos, vehículos y
        mecánicos para poder crear órdenes.
      </p>
      <nav className="catalog-nav" aria-label="Datos del taller">
        {Object.entries(catalogs)
          .filter(([key]) => !adminOnly(key) || session?.role === "Admin")
          .map(([key, item]) => (
            <Link
              key={key}
              className={`button ${key === catalog ? "" : "secondary"}`}
              to={`/datos/${key}`}
              aria-current={key === catalog ? "page" : undefined}
            >
              {item.title}
            </Link>
          ))}
      </nav>
      {config ? (
        <CatalogEditor key={catalog} config={config} />
      ) : (
        <p>Selecciona los datos que quieres consultar o registrar.</p>
      )}
    </>
  );
}
function CatalogEditor({ config }: { config: Catalog }) {
  const { session } = useAuth();
  const [deleting, setDeleting] = useState<Row | null>(null);
  const canDelete = config.endpoint !== "clientes" || session?.role === "Admin";
  const visibleFields = config.fields.filter((f) => f.type !== "password");
  const cache = useQueryClient();
  const [editing, setEditing] = useState<Row | null>(null);
  const [version, setVersion] = useState(0);
  const [success, setSuccess] = useState("");
  const rows = useQuery({
    queryKey: ["catalog", config.endpoint],
    queryFn: () => load(config),
  });
  const related = useQuery({
    queryKey: ["catalog-relations", config.endpoint],
    queryFn: async (): Promise<Record<string, Row[]>> =>
      Object.fromEntries(
        await Promise.all(
          config.fields
            .filter((f) => f.relation)
            .map(async (f) => [
              f.relation!,
              await load(relations[f.relation!]),
            ]),
        ),
      ),
  });
  const mutation = useMutation({
    mutationFn: (values: Row) =>
      request(`/${config.endpoint}/${editing ? editing[config.id] : ""}`, {
        method: editing ? "PUT" : "POST",
        body: JSON.stringify(values),
      }),
    onSuccess: async () => {
      setSuccess(editing ? "Cambios guardados." : "Registro creado.");
      setEditing(null);
      setVersion((v) => v + 1);
      await cache.invalidateQueries();
    },
  });
  const deletion = useMutation({
    mutationFn: (row: Row) =>
      request(`/${config.endpoint}/${row[config.id]}`, { method: "DELETE" }),
    onSuccess: async () => {
      if (editing?.[config.id] === deleting?.[config.id]) {
        setEditing(null);
        setVersion((v) => v + 1);
      }
      setDeleting(null);
      setSuccess("Registro eliminado.");
      await cache.invalidateQueries();
    },
  });
  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    const values: Row = {};
    for (const field of config.fields) {
      const value = String(data.get(field.name) || "").trim();
      values[field.name] =
        field.relation || field.type === "number"
          ? value
            ? Number(value)
            : null
          : field.type === "datetime-local"
            ? value || null
            : value;
      if (field.type === "password")
        values[field.name] = String(data.get(field.name) || "") || null;
    }
    setSuccess("");
    mutation.mutate(values);
  }
  const missing = config.fields.filter(
    (f) => f.relation && related.data && !related.data[f.relation!]?.length,
  );
  return (
    <>
      <h2>{config.title}</h2>
      <Errors error={rows.error || related.error} />
      {(rows.isError || related.isError) && (
        <button
          onClick={() => {
            void rows.refetch();
            void related.refetch();
          }}
        >
          Reintentar
        </button>
      )}
      {rows.isPending || related.isPending ? (
        <p role="status">Cargando datos…</p>
      ) : (
        <>
          <form className="panel editor" key={version} onSubmit={submit}>
            <h3>{editing ? "Editar registro" : "Nuevo registro"}</h3>
            <p className="muted">
              Los campos marcados como opcionales pueden quedar vacíos.
            </p>
            {config.endpoint === "usuarios" && (
              <p className="notice">
                Selecciona el rol Admin o Mecánico. Al editar, deja la
                contraseña vacía para conservarla. Los cambios de rol requieren
                volver a iniciar sesión para actualizar los permisos del token.
              </p>
            )}
            {missing.map((f) => (
              <p key={f.name} className="notice">
                Primero registra{" "}
                <Link
                  to={
                    f.relation === "ordenes"
                      ? "/ordenes"
                      : `/datos/${f.relation}`
                  }
                >
                  {relations[f.relation!].title.toLowerCase()}
                </Link>
                .
              </p>
            ))}
            <div className="form-grid">
              {config.fields.map((f) => (
                <div key={f.name}>
                  <label htmlFor={`catalog-${f.name}`}>
                    {f.label}
                    {f.optional || (f.type === "password" && editing)
                      ? " (opcional)"
                      : ""}
                  </label>
                  {f.relation ? (
                    <select
                      id={`catalog-${f.name}`}
                      name={f.name}
                      defaultValue={editing?.[f.name] ?? ""}
                      required
                      disabled={mutation.isPending}
                    >
                      <option value="">Seleccionar</option>
                      {related.data?.[f.relation]
                        ?.filter(
                          (row) =>
                            config.endpoint !== "usuarios" ||
                            f.relation !== "roles" ||
                            ["Admin", "Mecanico"].includes(String(row.nombre)),
                        )
                        .map((row) => (
                          <option
                            key={String(row[relations[f.relation!].id])}
                            value={String(row[relations[f.relation!].id])}
                          >
                            {label(row)}
                          </option>
                        ))}
                    </select>
                  ) : (
                    <input
                      id={`catalog-${f.name}`}
                      name={f.name}
                      type={f.type || "text"}
                      defaultValue={
                        f.type === "datetime-local"
                          ? String(editing?.[f.name] || "").slice(0, 16)
                          : (editing?.[f.name] ?? "")
                      }
                      required={
                        !f.optional && !(f.type === "password" && editing)
                      }
                      minLength={
                        f.type === "password"
                          ? 6
                          : config.endpoint === "usuarios" &&
                              f.name === "nombre"
                            ? 3
                            : undefined
                      }
                      autoComplete={
                        f.type === "password" ? "new-password" : undefined
                      }
                      maxLength={f.max}
                      min={f.type === "number" ? (f.min ?? 1) : undefined}
                      step={f.type === "number" ? (f.step ?? 1) : undefined}
                      disabled={mutation.isPending}
                    />
                  )}
                </div>
              ))}
            </div>
            <Errors error={mutation.error} />
            <div className="actions">
              <button
                disabled={
                  mutation.isPending || !!missing.length || related.isError
                }
                type="submit"
              >
                {mutation.isPending ? "Guardando…" : "Guardar registro"}
              </button>
              {editing && (
                <button
                  type="button"
                  className="secondary"
                  disabled={mutation.isPending}
                  onClick={() => {
                    setEditing(null);
                    setVersion((v) => v + 1);
                    mutation.reset();
                  }}
                >
                  Cancelar edición
                </button>
              )}
            </div>
          </form>
          {success && (
            <p className="notice" role="status">
              {success}
            </p>
          )}
          <section
            className="panel catalog-list"
            aria-label={`Listado de ${config.title.toLowerCase()}`}
          >
            {!rows.data?.length ? (
              <p>No hay registros todavía.</p>
            ) : (
              <table>
                <thead>
                  <tr>
                    {visibleFields.map((f) => (
                      <th key={f.name} scope="col">
                        {f.label}
                      </th>
                    ))}
                    <th scope="col">Acciones</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.data?.map((row) => (
                    <tr key={String(row[config.id])}>
                      {visibleFields.map((f) => (
                        <td key={f.name}>
                          {f.relation
                            ? label(
                                related.data?.[f.relation]?.find(
                                  (r) =>
                                    r[relations[f.relation!].id] ===
                                    row[f.name],
                                ) || {},
                              ) || "Sin referencia"
                            : (row[f.name] ?? "—")}
                        </td>
                      ))}
                      <td>
                        <button
                          className="quiet"
                          disabled={mutation.isPending}
                          onClick={() => {
                            setEditing(row);
                            setVersion((v) => v + 1);
                            setSuccess("");
                            mutation.reset();
                            document.querySelector("h2")?.scrollIntoView?.();
                          }}
                        >
                          Editar {label(row)}
                        </button>
                        {canDelete && (
                          <button
                            className="danger"
                            disabled={mutation.isPending || deletion.isPending}
                            onClick={() => {
                              setDeleting(row);
                              deletion.reset();
                            }}
                          >
                            Eliminar {label(row)}
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </section>
          {deleting && (
            <dialog
              className="panel"
              ref={(node) => {
                if (node && !node.open) node.showModal();
              }}
              onCancel={(event) => {
                if (deletion.isPending) event.preventDefault();
                else setDeleting(null);
              }}
              role="alertdialog"
              aria-modal="true"
              aria-labelledby="delete-catalog-title"
            >
              <h2 id="delete-catalog-title">Confirmar eliminación</h2>
              <p>
                ¿Eliminar {label(deleting)}? Esta acción no se puede deshacer.
                Si tiene registros relacionados, la API puede impedir la
                eliminación.
              </p>
              <Errors error={deletion.error} />
              <div className="actions">
                <button
                  className="danger"
                  disabled={deletion.isPending}
                  onClick={() => deletion.mutate(deleting)}
                >
                  {deletion.isPending ? "Eliminando…" : "Confirmar eliminación"}
                </button>
                <button
                  autoFocus
                  disabled={deletion.isPending}
                  onClick={() => setDeleting(null)}
                >
                  Cancelar
                </button>
              </div>
            </dialog>
          )}
        </>
      )}
    </>
  );
}
