# Prueba real de las API con rol Admin

Fecha: 20 de octubre de 2026. API local: `https://localhost:7027/api/v1`.

Se utilizó el token Admin suministrado por el usuario y la API en ejecución conectada a SQL Server. No se utilizaron respuestas HTTP simuladas. No se modificaron registros previos a la prueba. El certificado HTTPS local se aceptó sin verificar en el cliente de prueba; esto no prueba su confianza en un navegador.

## Resultado por recurso

| Recurso / ruta | Crear | Editar | Consultar por ID | Listar y comprobar cambios | Eliminar y comprobar ausencia |
| --- | --- | --- | --- | --- | --- |
| Roles `/rol` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Clientes `/clientes` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Marcas `/marca` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Modelos `/modelo` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Especialidades `/especialidad` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Mecánicos `/mecanicos` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Vehículos `/vehiculos` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Servicios `/servicios` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Repuestos `/repuestos` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Órdenes `/ordenes-trabajo` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |
| Pagos `/pago` | 201 | 204 | 200 | OK | 204 + 404 + ausencia en listado |

También se probó `/usuarios/registro` (201) y `/usuarios/login` (200) con una cuenta nueva, cuya contraseña aleatoria se mantuvo únicamente en memoria. Estos endpoints son anónimos y no disponen de listado, edición ni eliminación de usuarios. Las tablas de detalles de servicios/repuestos no tienen endpoints propios en el proyecto actual.

Se completaron 126 comprobaciones satisfactorias. Durante la primera ejecución se recibió un 429 por el límite global de 100 solicitudes/minuto; se continuó desde el punto pendiente tras una pausa y la eliminación respondió 204. El intento original queda conservado en la evidencia JSON.

## Datos conservados

Se conserva un conjunto relacionado con identificador `QA261020224749V`: rol ID 3 y cliente, marca, modelo, especialidad, mecánico, vehículo, servicio, repuesto, orden y pago con ID 1. Los nombres incluyen el prefijo QA; los campos editados incluyen `editado`. No son datos reales del taller.

La cuenta de prueba de registro es `QA261020224858Usuario`, ID 4, con rol Mecánico. No se ha guardado su contraseña. No existe API para borrarla o restablecerla.

El segundo conjunto de 11 registros, con identificador `QA261020224749D`, se eliminó completamente en orden inverso a sus dependencias. No se eliminaron los roles Admin ni Mecanico.

Los clientes, marcas, modelos, vehículos, especialidades y mecánicos se pueden consultar en **Datos del taller**; la orden en **Órdenes de trabajo**. Los listados de roles, servicios, repuestos y pagos se consultan mediante sus GET en Scalar: todavía no hay pantallas para ellos. Esta ejecución verificó los listados HTTP, no realizó una prueba visual del navegador.

## Hallazgo funcional

Crear un pago con estado `Completado` modifica automáticamente la orden y deja su estado como `Completado`. El frontend y la validación de órdenes esperan `Completada`. Por tanto, aunque el CRUD HTTP funcionó, el flujo pago/orden tiene una inconsistencia: la orden no coincide con el filtro y el contador de completadas. Además, PUT de pago no sincroniza el estado de la orden. El hallazgo se reporta sin cambiar el código de negocio ni los datos resultantes de esta prueba.

## Evidencia y límites

`PRUEBA_ADMIN_REAL.json` contiene métodos, rutas, estados, comprobaciones e IDs creados/eliminados; no contiene tokens ni contraseñas. `scripts/test_admin_live.py` ejecuta la prueba y admite `--resume` para continuar las eliminaciones pendientes de su último informe. Una nueva ejecución completa crea nuevos datos.

La prueba cubre operaciones válidas en las rutas expuestas; no demuestra que todos los casos de validación, concurrencia, referencias inválidas o autorización de otros roles funcionen correctamente. La persistencia se comprobó mediante nuevas lecturas de la API, sin consulta SQL directa independiente.
