using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class RepuestoApi
    {
        public static void MapRepuestoApi(this WebApplication app)
        {
            var repuesto = app.MapVersionedV1Group("repuestos", "Repuestos");
            
            repuesto.MapGet("/", async([Microsoft.AspNetCore.Mvc.FromServices] IRepuestoRepository repository) => await repository.GetAllAsync());

            repuesto.MapGet("/{id}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IRepuestoRepository repository) =>
            {
                var r = await repository.GetByIdAsync(id);
                return r is not null ? Results.Ok(r) : Results.NotFound();
            });

            repuesto.MapPost("/", async (RepuestoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IRepuestoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var r = new Repuesto
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Precio = dto.Precio,
                    Stock = dto.Stock
                };

                await repository.AddAsync(r);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/repuestos/{r.RepuestoId}", r);
            });
            
            repuesto.MapPut("/{id}", async (int id, RepuestoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IRepuestoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var r = await repository.GetByIdAsync(id);
                if (r is null) return Results.NotFound();
                
                r.Nombre = dto.Nombre;
                r.Descripcion = dto.Descripcion;
                r.Precio = dto.Precio;
                r.Stock = dto.Stock;
                
                repository.Update(r);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            repuesto.MapDelete("/{id}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IRepuestoRepository repository) =>
            {
                var r = await repository.GetByIdAsync(id);
                if (r is null) return Results.NotFound();
                repository.Remove(r);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


