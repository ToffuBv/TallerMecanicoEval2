using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class EspecialidadApi
    {
        public static void MapEspecialidadApi(this WebApplication app)
        {
            var especialidades = app.MapVersionedV1Group("especialidad", "Especialidad");

            especialidades.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IEspecialidadRepository repository) => await repository.GetAllAsync());

            especialidades.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IEspecialidadRepository repository) =>
            {
                var especialidad = await repository.GetByIdAsync(id);
                return especialidad is not null ? Results.Ok(especialidad) : Results.NotFound();
            });

            especialidades.MapPost("/", async (EspecialidadDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IEspecialidadRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var especialidad = new Especialidade
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion
                };

                await repository.AddAsync(especialidad);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/especialidad/{especialidad.EspecialidadId}", especialidad);
            });

            especialidades.MapPut("/{id:int}", async (int id, EspecialidadDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IEspecialidadRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var existingEspecialidad = await repository.GetByIdAsync(id);
                if (existingEspecialidad is null) return Results.NotFound();
                
                existingEspecialidad.Nombre = dto.Nombre;
                existingEspecialidad.Descripcion = dto.Descripcion;
                
                repository.Update(existingEspecialidad);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            especialidades.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IEspecialidadRepository repository) =>
            {
                var existingEspecialidad = await repository.GetByIdAsync(id);
                if (existingEspecialidad is null) return Results.NotFound();
                
                repository.Remove(existingEspecialidad);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


