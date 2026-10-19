using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class MarcaApi
    {
        public static void MapMarcaApi(this WebApplication app)
        {
            var marca = app.MapVersionedV1Group("marca", "Marca");
            
            marca.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IMarcaRepository repository) => await repository.GetAllAsync());

            marca.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IMarcaRepository repository) =>
            {
                var m = await repository.GetByIdAsync(id);
                return m is not null ? Results.Ok(m) : Results.NotFound();
            });

            marca.MapPost("/", async (MarcaDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IMarcaRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var m = new Marca
                {
                    Nombre = dto.Nombre
                };

                await repository.AddAsync(m);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/marca/{m.MarcaId}", m);
            });

            marca.MapPut("/{id:int}", async (int id, MarcaDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IMarcaRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var m = await repository.GetByIdAsync(id);
                if (m is null) return Results.NotFound();
                m.Nombre = dto.Nombre;
                repository.Update(m);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            marca.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IMarcaRepository repository) =>
            {
                var m = await repository.GetByIdAsync(id);
                if (m is null) return Results.NotFound();
                repository.Remove(m);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


