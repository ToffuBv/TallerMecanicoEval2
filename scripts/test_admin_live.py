"""Prueba manual autorizada contra la API local y SQL Server reales.

Lee un token Admin de Frontend/.admin-test-token sin imprimirlo.
Crea un conjunto persistente QA y otro descartable; solo borra sus propios IDs.
No modifica usuarios, roles ni registros existentes.
"""
import json
import secrets
import ssl
import sys
import time
from datetime import datetime
from pathlib import Path
from urllib.request import Request, urlopen
from urllib.error import HTTPError

ROOT = Path(__file__).resolve().parents[1]
BASE = "https://localhost:7027/api/v1"
TOKEN_FILE = ROOT / "Frontend/.admin-test-token"
REPORT = ROOT / "docs/PRUEBA_ADMIN_REAL.json"
# Solo para el certificado de desarrollo del servidor local.
TLS = ssl._create_unverified_context()
results = []
created = []


def call(method, path, payload=None, token=None):
    headers = {"Accept": "application/json"}
    if token:
        headers["Authorization"] = "Bearer " + token
    body = None
    if payload is not None:
        headers["Content-Type"] = "application/json"
        body = json.dumps(payload).encode()
    req = Request(BASE + path, data=body, method=method, headers=headers)
    try:
        with urlopen(req, context=TLS, timeout=20) as response:
            raw = response.read()
            return response.status, json.loads(raw) if raw else None
    except HTTPError as error:
        raw = error.read()
        try:
            data = json.loads(raw)
        except ValueError:
            data = {"message": "Respuesta no JSON; revisar consola de la API."}
        return error.code, data


def check(method, path, expected, payload=None, verify=None, token=None):
    status, data = call(method, path, payload, token)
    for _ in range(3):
        if status != 429:
            break
        results.append({"method": method, "path": path, "status": 429, "observation": "Límite de peticiones; reintento tras pausa"})
        print("Límite de peticiones activo; pausa de 25 segundos.", flush=True)
        time.sleep(25)
        status, data = call(method, path, payload, token)
    passed = status == expected and (verify is None or verify(data))
    entry = {"method": method, "path": path, "status": status, "expected": expected, "passed": passed}
    # No persistir tokens, contraseñas ni datos de otros usuarios.
    if not passed and isinstance(data, dict):
        entry["errors"] = data.get("errors", data.get("title", "Verificar respuesta del servidor"))
    results.append(entry)
    print(f'{"OK" if passed else "FALLO"} {method} {path}: {status}', flush=True)
    if not passed:
        raise RuntimeError(f"Falló {method} {path}")
    return data


def test_registration(run):
    password = secrets.token_urlsafe(24)
    username = "QA" + run + "Usuario"
    user = check("POST", "/usuarios/registro", 201, {"nombre": username, "correo": username + "@example.com", "password": password})
    created.append({"route": "usuarios", "id": user["usuarioId"], "tag": username, "deleted": False})
    check("POST", "/usuarios/login", 200, {"nombreUsuario": username, "password": password}, verify=lambda row: isinstance(row.get("token"), str))


def main():
    if not TOKEN_FILE.exists():
        print("PENDIENTE: guardar token Admin en Frontend/.admin-test-token; no se enviaron solicitudes.")
        return 2
    token = TOKEN_FILE.read_text(encoding="utf-8-sig").strip()
    if token.startswith("Bearer "):
        token = token[7:]
    if "--resume" in sys.argv:
        previous = json.loads(REPORT.read_text(encoding="utf-8"))
        results.extend(previous["results"])
        created.extend(previous["created"])
        id_keys = {"rol": "rolId", "clientes": "clienteId", "marca": "marcaId", "modelo": "modeloId", "especialidad": "especialidadId", "mecanicos": "mecanicoId", "vehiculos": "vehiculoId", "servicios": "servicioId", "repuestos": "repuestoId", "ordenes-trabajo": "ordenId", "pago": "pagoId"}
        check("GET", "/rol/", 200, token=token)
        for tracking in reversed(created):
            if not tracking["tag"].endswith("D") or tracking["deleted"]:
                continue
            route, ident = tracking["route"], tracking["id"]
            check("DELETE", f"/{route}/{ident}", 204, token=token)
            tracking["deleted"] = True
            check("GET", f"/{route}/{ident}", 404, token=token)
            check("GET", f"/{route}/", 200, verify=lambda rows, k=id_keys[route], i=ident: all(r.get(k) != i for r in rows), token=token)
        test_registration(datetime.now().strftime("%y%m%d%H%M%S"))
        return 0
    # Comprobación real de autenticación antes de crear datos.
    check("GET", "/rol/", 200, token=token)
    run = datetime.now().strftime("%y%m%d%H%M%S")
    for disposable in (False, True):
        tag = "QA" + run + ("D" if disposable else "V")
        ids = {}
        entities = [
            ("rol", "rolId", lambda: {"nombre": tag + " Rol", "descripcion": "Rol de prueba sin usuarios"}),
            ("clientes", "clienteId", lambda: {"nombre": tag, "apellido": "Cliente", "correo": tag + "@example.com", "telefono": "912345678", "direccion": "Dirección QA"}),
            ("marca", "marcaId", lambda: {"nombre": tag + " Marca"}),
            ("modelo", "modeloId", lambda: {"nombre": tag + " Modelo", "marcaId": ids["marca"]}),
            ("especialidad", "especialidadId", lambda: {"nombre": tag + " Especialidad", "descripcion": "Especialidad QA"}),
            ("mecanicos", "mecanicoId", lambda: {"nombre": tag, "apellido": "Mecánico", "telefono": "912345678", "especialidadId": ids["especialidad"]}),
            ("vehiculos", "vehiculoId", lambda: {"patente": tag, "anio": 2020, "color": "Azul", "clienteId": ids["clientes"], "modeloId": ids["modelo"]}),
            ("servicios", "servicioId", lambda: {"nombre": tag + " Servicio", "descripcion": "Servicio QA", "costo": 25000}),
            ("repuestos", "repuestoId", lambda: {"nombre": tag + " Repuesto", "descripcion": "Repuesto QA", "precio": 10000, "stock": 5}),
            ("ordenes-trabajo", "ordenId", lambda: {"fechaIngreso": "2026-10-20T12:00:00", "estado": "Pendiente", "observaciones": tag, "clienteId": ids["clientes"], "vehiculoId": ids["vehiculos"], "mecanicoId": ids["mecanicos"]}),
            ("pago", "pagoId", lambda: {"ordenId": ids["ordenes-trabajo"], "montoTotal": 35000, "metodoPago": "Efectivo", "fechaPago": "2026-10-20T12:00:00", "estado": "Completado"}),
        ]
        batch = []
        for route, key, factory in entities:
            payload = factory()
            record = check("POST", f"/{route}/", 201, payload, token=token)
            ident = record[key]
            ids[route] = ident
            tracking = {"route": route, "id": ident, "tag": tag, "deleted": False}
            created.append(tracking)
            batch.append((route, key, ident, tracking))
            edited = dict(payload)
            field = next((f for f in ("nombre", "observaciones", "color", "metodoPago") if f in edited), None)
            edited[field] += " editado"
            check("PUT", f"/{route}/{ident}", 204, edited, token=token)
            check("GET", f"/{route}/{ident}", 200, verify=lambda row, f=field, value=edited[field]: row.get(f) == value, token=token)
            check("GET", f"/{route}/", 200, verify=lambda rows, k=key, i=ident, f=field, value=edited[field]: any(r.get(k) == i and r.get(f) == value for r in rows), token=token)
        if disposable:
            for route, key, ident, tracking in reversed(batch):
                check("DELETE", f"/{route}/{ident}", 204, token=token)
                tracking["deleted"] = True
                check("GET", f"/{route}/{ident}", 404, token=token)
                check("GET", f"/{route}/", 200, verify=lambda rows, k=key, i=ident: all(r.get(k) != i for r in rows), token=token)
        else:
            # Pago modifica el estado de la orden; registrar ese efecto real.
            order = check("GET", f'/ordenes-trabajo/{ids["ordenes-trabajo"]}', 200, token=token)
            results.append({"observation": "Estado de orden después de crear pago", "value": order.get("estado")})
    # Usuarios solo dispone de registro y login: no hay listado, edición ni borrado.
    test_registration(run)
    return 0


if __name__ == "__main__":
    exit_code = 1
    try:
        exit_code = main()
    except Exception as error:
        print(str(error), file=sys.stderr)
    finally:
        if results or created:
            REPORT.write_text(json.dumps({"timestamp": datetime.now().isoformat(), "base": BASE, "complete": exit_code == 0, "results": results, "created": created}, ensure_ascii=False, indent=2), encoding="utf-8")
    sys.exit(exit_code)
