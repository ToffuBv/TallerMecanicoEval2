# Verificar persistencia y logs

El 20 de octubre de 2026 se repitió la prueba de Chromium de las vistas nuevas con `TALLER_KEEP_TEST_DATA=1`, conservando los datos. Las operaciones se realizaron desde los formularios y las respuestas de datos de la API no fueron simuladas. La sesión se incorporó con el JWT real de Admin, sin volver a probar el login.

Después de crear y editar se consultó SQL Server directamente usando sqlcmd con autenticación de Windows, de forma independiente de la API. El archivo `EVIDENCIA_SQL_VISTAS.txt` contiene el resultado:

- Servicio 4: `UIQA1792548424269Servicio editado`, costo 1234.50.
- Repuesto 4: `UIQA1792548424269Repuesto editado`, precio 123.45, stock 0.
- Rol 6: `UIQA1792548424269Rol editado`, sin usuarios asociados.
- Orden 4: `UIQA1792548424269 Orden pago`.
- Pago 4 de la orden 4: monto editado 3456.78, estado Pendiente.

Estos registros se dejaron en SQL Server para su revisión desde los listados. La prueba de eliminación de los cuatro recursos ya se había completado en la ejecución anterior; esta repetición omitió eliminar para conservar evidencia.

## Consulta independiente en SSMS

Abre `scripts/verificar_datos_prueba.sql` en SSMS, conéctate a `.\SQLEXPRESS` y ejecútalo. Es de solo lectura y muestra los registros UIQA. Puedes volver a ejecutarlo después de editar desde las vistas para comparar los cambios.

## Logs de consultas

En desarrollo se agregó el nivel Information para la categoría `Microsoft.EntityFrameworkCore.Database.Command`. Reinicia la API en Visual Studio para cargarlo.

Consulta la consola donde se ejecuta la API o **Ver → Salida**, seleccionando la salida de depuración/ASP.NET Core correspondiente a su modo de ejecución. Al guardar o listar desde la aplicación, busca `Microsoft.EntityFrameworkCore.Database.Command` y `Executed DbCommand`. Podrás ver instrucciones INSERT, UPDATE, DELETE y SELECT emitidas por Entity Framework. Los valores sensibles de los parámetros no se habilitaron.

Los mensajes se envían a los proveedores de logging configurados, normalmente consola y depuración. No se ha agregado un archivo persistente de logs ni una auditoría de SQL Server. Si necesitas conservar una demostración, copia la salida de Visual Studio y compárala con los resultados de las consultas SQL.

Una instrucción registrada no demuestra por sí sola que una transacción terminó confirmada. La consulta independiente posterior de las filas y sus valores es la evidencia de persistencia utilizada aquí. El encabezado Network del navegador demuestra la petición HTTP, no la escritura SQL por sí solo.
