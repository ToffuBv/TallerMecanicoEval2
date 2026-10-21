# Prueba de las nuevas vistas

Fecha: 20 de octubre de 2026.

Se completaron las vistas de Servicios, Repuestos, Pagos y Roles en **Datos del taller**, con listado, creación, edición y eliminación confirmada. Se agregó eliminación a los seis catálogos existentes; el botón para eliminar clientes se muestra únicamente a Admin. La API conserva sus reglas de autorización y validación.

La prueba `Frontend/e2e/catalogs-live.spec.ts` se ejecutó en Chromium contra el frontend, la API local y SQL Server. Resultado: **1 prueba aprobada**, que recorre las cuatro vistas nuevas. Se comprobaron importes decimales, stock cero, selección de orden, fecha de pago y confirmación de eliminación.

Para incorporar la sesión al navegador automatizado se suministró el JWT real de Admin mediante una respuesta de login interceptada. El login no se volvió a probar en esta ejecución. Las operaciones de datos de las cuatro vistas se realizaron desde los formularios contra las API reales, sin simular sus respuestas. Se verificó creación, cambios en los listados y desaparición después de eliminar.

Se crearon y eliminaron un servicio, un repuesto, un rol de prueba y un pago. Para probar el pago se creó desde la interfaz una orden propia usando el cliente, vehículo y mecánico QA con ID 1. Esa orden se conserva, con observaciones `UIQA… Orden pago` y sin pago asociado después del borrado de prueba. Los registros existentes no se modificaron.

La ejecución real es opcional: requiere la variable `TALLER_LIVE_TEST=1`, la API iniciada, un token Admin vigente en `.admin-test-token` y los registros relacionados QA ID 1. Sin esa variable, la prueba se omite. Usa `--trace=off` para esta ejecución con credenciales reales. No guarda el token en el código ni en este informe.

La inconsistencia entre `Completado` y `Completada` detectada previamente no se modificó, por indicación del usuario. Las tablas de detalles de servicios y repuestos no tienen endpoints propios y no se agregó una pantalla que simule operaciones inexistentes. Usuarios dispone de las vistas de registro/login; la API actual no ofrece listado, edición ni eliminación de usuarios.
