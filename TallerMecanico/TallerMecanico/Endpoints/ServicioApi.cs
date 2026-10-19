using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class ServicioApi
    {
        public static void MapServicioApi(this WebApplication app)
        {
            var servicio = app.MapVersionedV1Group("servicios", "Servicios");

            servicio.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IServicioRepository repository) => await repository.GetAllAsync());

            servicio.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IServicioRepository repository) =>
            {
                var s = await repository.GetByIdAsync(id);
                return s is not null ? Results.Ok(s) : Results.NotFound();
            });

            servicio.MapPost("/", async (ServicioDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IServicioRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var s = new Servicio
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Costo = dto.Costo
                };

                await repository.AddAsync(s);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/servicios/{s.ServicioId}", s);
            });

            servicio.MapPut("/{id:int}", async (int id, ServicioDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IServicioRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var s = await repository.GetByIdAsync(id);
                if (s is null) return Results.NotFound();
                
                s.Nombre = dto.Nombre;
                s.Descripcion = dto.Descripcion;
                s.Costo = dto.Costo;
                
                repository.Update(s);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            servicio.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IServicioRepository repository) =>
            {
                var s = await repository.GetByIdAsync(id);
                if (s is null) return Results.NotFound();
                repository.Remove(s);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


