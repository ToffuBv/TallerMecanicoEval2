USE TallerMecanicoDB;
GO
SET NOCOUNT ON;

SELECT @@SERVERNAME AS servidor, DB_NAME() AS base_datos;

SELECT servicio_id, nombre, costo
FROM dbo.servicios WHERE nombre LIKE N'UIQA%';

SELECT repuesto_id, nombre, precio, stock
FROM dbo.repuestos WHERE nombre LIKE N'UIQA%';

SELECT rol_id, nombre, descripcion
FROM dbo.roles WHERE nombre LIKE N'UIQA%';

SELECT o.orden_id, o.observaciones, o.estado,
       p.pago_id, p.monto_total, p.metodo_pago, p.estado AS estado_pago
FROM dbo.ordenes_trabajo o
LEFT JOIN dbo.pagos p ON p.orden_id = o.orden_id
WHERE o.observaciones LIKE N'UIQA%';
