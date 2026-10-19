import {
  useEffect,
  useRef,
  useState,
  type FormEvent,
  type ReactNode,
} from "react";
import {
  Link,
  Navigate,
  Outlet,
  Route,
  Routes,
  useLocation,
  useNavigate,
  useParams,
} from "react-router-dom";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api, ApiError } from "./api";
import { useAuth } from "./auth";
import { inputSchema, type Order, type OrderInput } from "./contracts";

export function ProtectedRoute() {
  const { session } = useAuth();
  if (!session) return <Navigate to="/login" replace />;
  if (!["Admin", "Mecanico"].includes(session.role))
    return <Navigate to="/acceso-denegado" replace />;
  return <Outlet />;
}
function ErrorNotice({ error }: { error: unknown }) {
  return error ? (
    <div className="notice error" role="alert">
      {error instanceof Error ? error.message : "Ocurrió un error."}
    </div>
  ) : null;
}
function RouteFocus() {
  const location = useLocation();
  useEffect(() => {
    document.querySelector<HTMLElement>("h1")?.focus();
  }, [location.pathname]);
  return null;
}
function Layout() {
  const { session, logout } = useAuth();
  const navigate = useNavigate();
  useEffect(() => {
    const denied = () => navigate("/acceso-denegado");
    window.addEventListener("access-denied", denied);
    return () => window.removeEventListener("access-denied", denied);
  }, [navigate]);
  return (
    <>
      <a href="#contenido" className="skip">
        Saltar al contenido
      </a>
      <header>
        <Link to="/ordenes" className="brand">
          <span className="brand-icon" aria-hidden="true">
            T
          </span>{" "}
          TALLER<span className="brand-sub">Panel de gestión</span>
        </Link>
        {session && (
          <nav aria-label="Principal">
            <Link to="/ordenes">Órdenes de trabajo</Link>
            <span className="identity">
              {session.name} · {session.role}
            </span>
            <button className="quiet" onClick={logout}>
              Cerrar sesión
            </button>
          </nav>
        )}
      </header>
      <main id="contenido">
        <Outlet />
      </main>
      <footer>
        TALLER / Gestión mecánica{" "}
        <span>Lenguaje Programación Web II · 2026</span>
      </footer>
      <RouteFocus />
    </>
  );
}
export function Login() {
  const { login, session } = useAuth();
  const [error, setError] = useState<unknown>();
  const [busy, setBusy] = useState(false);
  if (session) return <Navigate to="/ordenes" replace />;
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    setBusy(true);
    setError(undefined);
    try {
      await login(
        String(data.get("usuario")).trim(),
        String(data.get("password")),
      );
    } catch (err) {
      setError(err);
    } finally {
      setBusy(false);
    }
  }
  return (
    <div className="login-grid">
      <section className="intro">
        <p className="eyebrow">CADA ORDEN, BAJO CONTROL</p>
        <h1 tabIndex={-1}>
          Más claridad.
          <br />
          Mejor servicio.
        </h1>
        <p>
          Organiza el trabajo de tu taller. Consulta vehículos, asigna mecánicos
          y acompaña cada reparación de principio a fin.
        </p>
        <div className="intro-note">01 / Un solo lugar para tu operación</div>
      </section>
      <section className="panel login-panel" aria-labelledby="login-title">
        <p className="eyebrow">BIENVENIDO A TU TALLER</p>
        <h2 id="login-title">Iniciar sesión</h2>
        <p className="muted">
          Ingresa con tu cuenta de administrador o mecánico.
        </p>
        <form onSubmit={submit}>
          <label htmlFor="usuario">Usuario</label>
          <input
            id="usuario"
            name="usuario"
            autoComplete="username"
            required
            maxLength={100}
          />
          <label htmlFor="password">Contraseña</label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            required
            maxLength={200}
          />
          <ErrorNotice error={error} />
          <button disabled={busy} type="submit">
            {busy ? "Ingresando…" : "Entrar al taller →"}
          </button>
        </form>
        <p className="small muted">
          Por seguridad, la sesión se cierra al recargar la página y después de
          5 minutos.
        </p>
      </section>
    </div>
  );
}
function Heading({
  title,
  text,
  action,
}: {
  title: string;
  text: string;
  action?: ReactNode;
}) {
  return (
    <div className="heading">
      <div>
        <p className="eyebrow">OPERACIÓN / ÓRDENES</p>
        <h1 tabIndex={-1}>{title}</h1>
        <p className="muted">{text}</p>
      </div>
      {action}
    </div>
  );
}
function Status({ value }: { value: string }) {
  return (
    <span className={`badge ${value === "Completada" ? "done" : ""}`}>
      {value}
    </span>
  );
}
function Orders() {
  const query = useQuery({ queryKey: ["orders"], queryFn: api.orders });
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");
  const orders = query.data || [];
  const filtered = orders.filter(
    (o) =>
      (!status || o.estado === status) &&
      `${o.ordenId} ${o.observaciones || ""} ${o.estado}`
        .toLowerCase()
        .includes(search.toLowerCase()),
  );
  return (
    <>
      <Heading
        title="Órdenes de trabajo"
        text="Una vista clara de lo que está pasando en tu taller."
        action={
          <Link className="button" to="/ordenes/nueva">
            + Nueva orden
          </Link>
        }
      />
      <div className="stats">
        <div>
          <span>Órdenes registradas</span>
          <strong>{orders.length}</strong>
        </div>
        <div>
          <span>En proceso</span>
          <strong>
            {orders.filter((o) => o.estado === "En proceso").length}
          </strong>
        </div>
        <div>
          <span>Completadas</span>
          <strong>
            {orders.filter((o) => o.estado === "Completada").length}
          </strong>
        </div>
      </div>
      <section className="panel">
        <div className="toolbar">
          <div>
            <label htmlFor="buscar">Buscar orden</label>
            <input
              id="buscar"
              type="search"
              placeholder="Número, estado u observaciones"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <div>
            <label htmlFor="estado-filtro">Estado</label>
            <select
              id="estado-filtro"
              value={status}
              onChange={(e) => setStatus(e.target.value)}
            >
              <option value="">Todos los estados</option>
              {["Pendiente", "En proceso", "Completada", "Cancelada"].map(
                (s) => (
                  <option key={s}>{s}</option>
                ),
              )}
            </select>
          </div>
        </div>
        <ErrorNotice error={query.error} />
        {query.isError && (
          <button onClick={() => query.refetch()}>Reintentar</button>
        )}
        {query.isPending ? (
          <p role="status">Cargando órdenes…</p>
        ) : (
          <>
            <p className="small muted" role="status">
              {filtered.length} órdenes encontradas
            </p>
            <div className="order-list">
              {filtered.map((o) => (
                <article className="order-row" key={o.ordenId}>
                  <div>
                    <span className="small muted">ORDEN</span>
                    <h2>
                      <Link to={`/ordenes/${o.ordenId}`}>
                        #{String(o.ordenId).padStart(4, "0")}
                      </Link>
                    </h2>
                  </div>
                  <div>
                    <span className="small muted">INGRESO</span>
                    <p>
                      {o.fechaIngreso
                        ? new Date(o.fechaIngreso).toLocaleDateString("es-CL")
                        : "Sin fecha"}
                    </p>
                  </div>
                  <div className="order-notes">
                    <p>{o.observaciones || "Sin observaciones"}</p>
                  </div>
                  <Status value={o.estado} />
                  <Link
                    className="row-link"
                    to={`/ordenes/${o.ordenId}`}
                    aria-label={`Ver orden ${o.ordenId}`}
                  >
                    Ver orden →
                  </Link>
                </article>
              ))}
            </div>
            {!filtered.length && (
              <div className="empty">
                <h2>
                  {orders.length
                    ? "Sin coincidencias"
                    : "Tu próxima reparación empieza aquí"}
                </h2>
                <p>
                  {orders.length
                    ? "Prueba con otra búsqueda o estado."
                    : "Crea una orden para organizar el trabajo de tu taller."}
                </p>
              </div>
            )}
          </>
        )}
      </section>
    </>
  );
}
function useRelated() {
  const clients = useQuery({ queryKey: ["clients"], queryFn: api.clients });
  const vehicles = useQuery({ queryKey: ["vehicles"], queryFn: api.vehicles });
  const mechanics = useQuery({
    queryKey: ["mechanics"],
    queryFn: api.mechanics,
  });
  return {
    clients,
    vehicles,
    mechanics,
    loading: clients.isPending || vehicles.isPending || mechanics.isPending,
    error: clients.error || vehicles.error || mechanics.error,
  };
}
function ConfirmDelete({
  order,
  close,
  confirm,
  busy,
  error,
}: {
  order: Order;
  close: () => void;
  confirm: () => void;
  busy: boolean;
  error: unknown;
}) {
  const ref = useRef<HTMLDialogElement>(null);
  useEffect(() => {
    const previous = document.activeElement as HTMLElement;
    ref.current?.showModal();
    return () => {
      previous?.focus();
    };
  }, []);
  return (
    <dialog
      ref={ref}
      onCancel={(e) => {
        e.preventDefault();
        if (!busy) close();
      }}
      aria-labelledby="delete-title"
      aria-describedby="delete-description"
    >
      <h2 id="delete-title">¿Eliminar orden #{order.ordenId}?</h2>
      <p id="delete-description">
        Esta acción elimina el registro de forma permanente.
      </p>
      <ErrorNotice error={error} />
      <div className="actions">
        <button className="secondary" autoFocus disabled={busy} onClick={close}>
          Cancelar
        </button>
        <button className="danger" disabled={busy} onClick={confirm}>
          {busy ? "Eliminando…" : "Eliminar orden"}
        </button>
      </div>
    </dialog>
  );
}
function Detail() {
  const { id } = useParams();
  const { session } = useAuth();
  const navigate = useNavigate();
  const cache = useQueryClient();
  const query = useQuery({
    queryKey: ["order", id],
    queryFn: () => api.order(id!),
  });
  const related = useRelated();
  const [deleting, setDeleting] = useState(false);
  const mutation = useMutation({
    mutationFn: () => api.remove(Number(id)),
    onSuccess: async () => {
      await cache.invalidateQueries({ queryKey: ["orders"] });
      navigate("/ordenes");
    },
  });
  if (query.isPending) return <p role="status">Cargando orden…</p>;
  if (!query.data)
    return (
      <>
        <Heading
          title="Orden no disponible"
          text="Vuelve al listado para seleccionar otra orden."
        />
        <ErrorNotice error={query.error} />
        <Link to="/ordenes">Volver al listado</Link>
      </>
    );
  const order = query.data;
  const client = related.clients.data?.find(
    (c) => c.clienteId === order.clienteId,
  );
  const vehicle = related.vehicles.data?.find(
    (v) => v.vehiculoId === order.vehiculoId,
  );
  const mechanic = related.mechanics.data?.find(
    (m) => m.mecanicoId === order.mecanicoId,
  );
  return (
    <>
      <Link className="back" to="/ordenes">
        ← Volver a las órdenes
      </Link>
      <Heading
        title={`Orden #${String(order.ordenId).padStart(4, "0")}`}
        text="Información y seguimiento de la reparación."
        action={
          <Link className="button" to={`/ordenes/${id}/editar`}>
            Editar orden
          </Link>
        }
      />
      <section className="panel detail">
        <Status value={order.estado} />
        <ErrorNotice error={related.error} />
        <dl>
          <div>
            <dt>Fecha de ingreso</dt>
            <dd>{order.fechaIngreso?.slice(0, 10) || "Sin fecha"}</dd>
          </div>
          <div>
            <dt>Cliente</dt>
            <dd>
              {client
                ? `${client.nombre} ${client.apellido}`
                : `Cliente #${order.clienteId}`}
            </dd>
          </div>
          <div>
            <dt>Vehículo</dt>
            <dd>{vehicle?.patente || `Vehículo #${order.vehiculoId}`}</dd>
          </div>
          <div>
            <dt>Mecánico asignado</dt>
            <dd>
              {mechanic
                ? `${mechanic.nombre} ${mechanic.apellido}`
                : `Mecánico #${order.mecanicoId}`}
            </dd>
          </div>
        </dl>
        <h2>Observaciones</h2>
        <p className="preserve">
          {order.observaciones || "Sin observaciones."}
        </p>
        {session?.role === "Admin" && (
          <button className="danger" onClick={() => setDeleting(true)}>
            Eliminar orden
          </button>
        )}
      </section>
      {deleting && (
        <ConfirmDelete
          order={order}
          close={() => setDeleting(false)}
          confirm={() => mutation.mutate()}
          busy={mutation.isPending}
          error={mutation.error}
        />
      )}
    </>
  );
}
function OrderForm() {
  const { id } = useParams();
  const related = useRelated();
  const order = useQuery({
    queryKey: ["order", id],
    queryFn: () => api.order(id!),
    enabled: !!id,
  });
  if (related.loading || (id && order.isPending))
    return <p role="status">Cargando formulario…</p>;
  if (related.error || order.error)
    return (
      <>
        <ErrorNotice error={related.error || order.error} />
        <Link to="/ordenes">Volver a las órdenes</Link>
      </>
    );
  return (
    <Editor key={id || "new"} id={id} initial={order.data} related={related} />
  );
}
function Editor({
  id,
  initial,
  related,
}: {
  id?: string;
  initial?: Order;
  related: ReturnType<typeof useRelated>;
}) {
  const navigate = useNavigate();
  const cache = useQueryClient();
  const [clientId, setClientId] = useState(String(initial?.clienteId || ""));
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const summary = useRef<HTMLDivElement>(null);
  const mutation = useMutation({
    mutationFn: (data: OrderInput) => api.save(data, id),
    onSuccess: async () => {
      await cache.invalidateQueries({ queryKey: ["orders"] });
      await cache.invalidateQueries({ queryKey: ["order", id] });
      navigate(id ? `/ordenes/${id}` : "/ordenes");
    },
    onError: (err) => {
      if (err instanceof ApiError)
        setErrors(
          Object.fromEntries(
            Object.entries(err.errors).map(([k, v]) => [
              k.charAt(0).toLowerCase() + k.slice(1),
              v,
            ]),
          ),
        );
    },
  });
  useEffect(() => {
    if (Object.keys(errors).length) summary.current?.focus();
  }, [errors]);
  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const values = Object.fromEntries(new FormData(event.currentTarget));
    const result = inputSchema.safeParse(values);
    if (!result.success) {
      setErrors(result.error.flatten().fieldErrors);
      return;
    }
    if (
      !related.vehicles.data?.some(
        (v) =>
          v.vehiculoId === result.data.vehiculoId &&
          v.clienteId === result.data.clienteId,
      )
    ) {
      setErrors({
        vehiculoId: ["El vehículo debe pertenecer al cliente seleccionado."],
      });
      return;
    }
    setErrors({});
    mutation.mutate(result.data);
  }
  const attrs = (name: string) => ({
    "aria-invalid": !!errors[name],
    "aria-describedby": errors[name] ? `${name}-error` : undefined,
  });
  const err = (name: string) =>
    errors[name] ? (
      <p className="field-error" id={`${name}-error`}>
        {errors[name].join(" ")}
      </p>
    ) : null;
  return (
    <>
      <Link className="back" to="/ordenes">
        ← Volver a las órdenes
      </Link>
      <Heading
        title={id ? "Editar orden" : "Nueva orden de trabajo"}
        text="Completa los datos de ingreso y asigna al responsable de la reparación."
      />
      <form className="panel editor" noValidate onSubmit={submit}>
        {Object.keys(errors).length > 0 && (
          <div
            className="notice error"
            role="alert"
            ref={summary}
            tabIndex={-1}
          >
            Revisa los campos indicados.{" "}
            {Object.entries(errors).map(([k, v]) => (
              <p key={k}>
                <a href={`#${k}`}>{v.join(" ")}</a>
              </p>
            ))}
          </div>
        )}
        <ErrorNotice error={mutation.error} />
        <h2>Datos de la orden</h2>
        <p className="muted">
          Todos los campos son obligatorios, excepto las observaciones.
        </p>
        <div className="form-grid">
          <div>
            <label htmlFor="clienteId">Cliente</label>
            <select
              id="clienteId"
              name="clienteId"
              required
              value={clientId}
              onChange={(e) => {
                setClientId(e.target.value);
                const select = document.getElementById(
                  "vehiculoId",
                ) as HTMLSelectElement;
                if (select) select.value = "";
              }}
              {...attrs("clienteId")}
            >
              <option value="">Seleccionar cliente</option>
              {related.clients.data?.map((c) => (
                <option key={c.clienteId} value={c.clienteId}>
                  {c.nombre} {c.apellido}
                </option>
              ))}
            </select>
            {err("clienteId")}
          </div>
          <div>
            <label htmlFor="vehiculoId">Vehículo del cliente</label>
            <select
              id="vehiculoId"
              name="vehiculoId"
              required
              defaultValue={initial?.vehiculoId || ""}
              {...attrs("vehiculoId")}
            >
              <option value="">Seleccionar vehículo</option>
              {related.vehicles.data
                ?.filter((v) => String(v.clienteId) === clientId)
                .map((v) => (
                  <option key={v.vehiculoId} value={v.vehiculoId}>
                    {v.patente}
                  </option>
                ))}
            </select>
            {err("vehiculoId")}
          </div>
          <div>
            <label htmlFor="mecanicoId">Mecánico asignado</label>
            <select
              id="mecanicoId"
              name="mecanicoId"
              required
              defaultValue={initial?.mecanicoId || ""}
              {...attrs("mecanicoId")}
            >
              <option value="">Seleccionar mecánico</option>
              {related.mechanics.data?.map((m) => (
                <option key={m.mecanicoId} value={m.mecanicoId}>
                  {m.nombre} {m.apellido}
                </option>
              ))}
            </select>
            {err("mecanicoId")}
          </div>
          <div>
            <label htmlFor="fechaIngreso">Fecha de ingreso</label>
            <input
              id="fechaIngreso"
              name="fechaIngreso"
              type="date"
              required
              defaultValue={
                initial?.fechaIngreso?.slice(0, 10) ||
                new Date().toLocaleDateString("en-CA")
              }
              {...attrs("fechaIngreso")}
            />
            {err("fechaIngreso")}
          </div>
          <div>
            <label htmlFor="estado">Estado</label>
            <select
              id="estado"
              name="estado"
              defaultValue={initial?.estado || "Pendiente"}
              {...attrs("estado")}
            >
              {["Pendiente", "En proceso", "Completada", "Cancelada"].map(
                (s) => (
                  <option key={s}>{s}</option>
                ),
              )}
            </select>
            {err("estado")}
          </div>
        </div>
        <label htmlFor="observaciones">Observaciones (opcional)</label>
        <textarea
          id="observaciones"
          name="observaciones"
          rows={4}
          maxLength={1000}
          defaultValue={initial?.observaciones || ""}
          {...attrs("observaciones")}
        />
        {err("observaciones")}
        <div className="actions">
          <Link
            className="button secondary"
            to={id ? `/ordenes/${id}` : "/ordenes"}
          >
            Cancelar
          </Link>
          <button disabled={mutation.isPending} type="submit">
            {mutation.isPending ? "Guardando…" : "Guardar orden"}
          </button>
        </div>
      </form>
    </>
  );
}
export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/login" element={<Login />} />
        <Route
          path="/acceso-denegado"
          element={
            <>
              <Heading
                title="Acceso denegado"
                text="Tu cuenta no tiene permiso para acceder a este recurso."
              />
              <Link to="/ordenes">Volver al inicio</Link>
            </>
          }
        />
        <Route element={<ProtectedRoute />}>
          <Route path="/ordenes" element={<Orders />} />
          <Route path="/ordenes/nueva" element={<OrderForm />} />
          <Route path="/ordenes/:id" element={<Detail />} />
          <Route path="/ordenes/:id/editar" element={<OrderForm />} />
        </Route>
        <Route path="/" element={<Navigate to="/ordenes" replace />} />
        <Route
          path="*"
          element={
            <>
              <Heading
                title="Página no encontrada"
                text="La dirección solicitada no está disponible."
              />
              <Link to="/ordenes">Volver al inicio</Link>
            </>
          }
        />
      </Route>
    </Routes>
  );
}
