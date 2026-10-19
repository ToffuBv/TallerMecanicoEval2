import { z } from 'zod';
export const orderSchema = z.object({ ordenId: z.number().int(), fechaIngreso: z.string().nullable(), estado: z.string(), observaciones: z.string().nullable(), clienteId: z.number(), vehiculoId: z.number(), mecanicoId: z.number() });
export type Order = z.infer<typeof orderSchema>;
export const clientSchema = z.object({ clienteId: z.number(), nombre: z.string(), apellido: z.string() });
export const vehicleSchema = z.object({ vehiculoId: z.number(), patente: z.string(), clienteId: z.number() });
export const mechanicSchema = z.object({ mecanicoId: z.number(), nombre: z.string(), apellido: z.string() });
export const inputSchema = z.object({ fechaIngreso: z.string().min(1, 'Selecciona una fecha.'), estado: z.enum(['Pendiente', 'En proceso', 'Completada', 'Cancelada']), observaciones: z.string().max(1000, 'Máximo 1000 caracteres.'), clienteId: z.coerce.number().int().positive('Selecciona un cliente.'), vehiculoId: z.coerce.number().int().positive('Selecciona un vehículo.'), mecanicoId: z.coerce.number().int().positive('Selecciona un mecánico.') });
export type OrderInput = z.infer<typeof inputSchema>;
