USE TallerMecanicoDB;
GO

-- Marcas
IF NOT EXISTS (SELECT * FROM marcas WHERE nombre = 'Toyota') INSERT INTO marcas (nombre) VALUES ('Toyota');
IF NOT EXISTS (SELECT * FROM marcas WHERE nombre = 'Ford') INSERT INTO marcas (nombre) VALUES ('Ford');

-- Modelos
IF NOT EXISTS (SELECT * FROM modelos WHERE nombre = 'Corolla') INSERT INTO modelos (nombre, marca_id) VALUES ('Corolla', (SELECT TOP 1 marca_id FROM marcas WHERE nombre='Toyota'));
IF NOT EXISTS (SELECT * FROM modelos WHERE nombre = 'Mustang') INSERT INTO modelos (nombre, marca_id) VALUES ('Mustang', (SELECT TOP 1 marca_id FROM marcas WHERE nombre='Ford'));

-- Vehiculos (Asume que los clientes 1 y 2 existen)
IF NOT EXISTS (SELECT * FROM vehiculos WHERE patente = 'AB123CD') INSERT INTO vehiculos (patente, modelo_id, cliente_id, color, anio) VALUES ('AB123CD', (SELECT TOP 1 modelo_id FROM modelos WHERE nombre='Corolla'), 1, 'Blanco', 2020);
IF NOT EXISTS (SELECT * FROM vehiculos WHERE patente = 'XY987ZT') INSERT INTO vehiculos (patente, modelo_id, cliente_id, color, anio) VALUES ('XY987ZT', (SELECT TOP 1 modelo_id FROM modelos WHERE nombre='Mustang'), 2, 'Rojo', 2021);

-- Especialidades
IF NOT EXISTS (SELECT * FROM especialidades WHERE nombre = 'Frenos') INSERT INTO especialidades (nombre, descripcion) VALUES ('Frenos', 'Especialista en frenos');
IF NOT EXISTS (SELECT * FROM especialidades WHERE nombre = 'Motor') INSERT INTO especialidades (nombre, descripcion) VALUES ('Motor', 'Reparacion general de motor');

-- Mecanicos
IF NOT EXISTS (SELECT * FROM mecanicos WHERE nombre = 'Pedro') INSERT INTO mecanicos (nombre, apellido, especialidad_id, telefono) VALUES ('Pedro', 'Martinez', (SELECT TOP 1 especialidad_id FROM especialidades WHERE nombre='Frenos'), '555-9999');
IF NOT EXISTS (SELECT * FROM mecanicos WHERE nombre = 'Luis') INSERT INTO mecanicos (nombre, apellido, especialidad_id, telefono) VALUES ('Luis', 'Garcia', (SELECT TOP 1 especialidad_id FROM especialidades WHERE nombre='Motor'), '555-8888');

-- OrdenesTrabajo
IF NOT EXISTS (SELECT * FROM ordenes_trabajo WHERE observaciones = 'Cambio de pastillas') 
INSERT INTO ordenes_trabajo (vehiculo_id, cliente_id, mecanico_id, fecha_ingreso, estado, observaciones) 
VALUES (
    (SELECT TOP 1 vehiculo_id FROM vehiculos WHERE patente='AB123CD'), 
    1, 
    (SELECT TOP 1 mecanico_id FROM mecanicos WHERE nombre='Pedro'), 
    '2026-09-05', 
    'Pendiente', 
    'Cambio de pastillas'
);
