using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class MecanicoApi
    {
        public static void MapMecanicoApi(this WebApplication app)
        {
            var mecanico = app.MapVersionedV1Group("mecanicos", "Mecanicos");

            mecanico.MapGet("/", async([Microsoft.AspNetCore.Mvc.FromServices] IMecanicoRepository repository) => await repository.GetAllAsync());

            mecanico.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IMecanicoRepository repository) =>
            {
                var mecanico = await repository.GetByIdAsync(id);
                return mecanico is not null ? Results.Ok(mecanico) : Results.NotFound();
            });

            mecanico.MapPost("/", async (MecanicoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IMecanicoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var mecanicoEntity = new Mecanico
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Telefono = dto.Telefono,
                    EspecialidadId = dto.EspecialidadId
                };

                await repository.AddAsync(mecanicoEntity);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/mecanicos/{mecanicoEntity.MecanicoId}", mecanicoEntity);
            });

            mecanico.MapPut("/{id:int}", async (int id, MecanicoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IMecanicoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var mecanicoEntity = await repository.GetByIdAsync(id);
                if (mecanicoEntity is null) return Results.NotFound();
                
                mecanicoEntity.Nombre = dto.Nombre;
                mecanicoEntity.Apellido = dto.Apellido;
                mecanicoEntity.Telefono = dto.Telefono;
                mecanicoEntity.EspecialidadId = dto.EspecialidadId;
                
                repository.Update(mecanicoEntity);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            mecanico.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IMecanicoRepository repository) =>
            {
                var mecanico = await repository.GetByIdAsync(id);
                if (mecanico is null) return Results.NotFound();
                repository.Remove(mecanico);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

        }
    }
}


