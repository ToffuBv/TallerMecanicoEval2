using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class PagoApi
    {
        public static void MapPagoApi(this WebApplication app)
        {
            var pago = app.MapVersionedV1Group("pago", "Pago");
            
            pago.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IPagoRepository repository) => await repository.GetAllAsync());
            
            pago.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IPagoRepository repository) =>
            {
                var p = await repository.GetByIdAsync(id);
                return p is not null ? Results.Ok(p) : Results.NotFound();
            });
            
            pago.MapPost("/", async (PagoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IPagoRepository repository, [Microsoft.AspNetCore.Mvc.FromServices] IOrdenesTrabajoRepository ordenesRepo) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var p = new Pago
                {
                    OrdenId = dto.OrdenId,
                    MontoTotal = dto.MontoTotal,
                    MetodoPago = dto.MetodoPago,
                    FechaPago = dto.FechaPago,
                    Estado = dto.Estado
                };

                await repository.AddAsync(p);
                await repository.SaveChangesAsync();

                // Actualizar automáticamente el estado de la orden de trabajo si el pago se completó
                var orden = await ordenesRepo.GetByIdAsync(dto.OrdenId);
                if (orden is not null && !string.IsNullOrWhiteSpace(dto.Estado))
                {
                    orden.Estado = dto.Estado;
                    ordenesRepo.Update(orden);
                    await ordenesRepo.SaveChangesAsync();
                }

                return Results.Created($"/api/v1/pago/{p.PagoId}", p);
            });
            
            pago.MapPut("/{id:int}", async (int id, PagoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IPagoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var p = await repository.GetByIdAsync(id);
                if (p is null) return Results.NotFound();
                
                p.OrdenId = dto.OrdenId;
                p.MontoTotal = dto.MontoTotal;
                p.MetodoPago = dto.MetodoPago;
                p.FechaPago = dto.FechaPago;
                p.Estado = dto.Estado;
                
                repository.Update(p);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
            
            pago.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IPagoRepository repository) =>
            {
                var p = await repository.GetByIdAsync(id);
                if (p is null) return Results.NotFound();
                repository.Remove(p);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


