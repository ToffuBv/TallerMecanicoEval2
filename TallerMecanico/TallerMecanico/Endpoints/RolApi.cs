using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class RolApi
    {
        public static void MapRolApi(this WebApplication app)
        {
            var rol = app.MapVersionedV1Group("rol", "Rol");

            rol.MapGet("/", async([Microsoft.AspNetCore.Mvc.FromServices] IRolRepository repository) => await repository.GetAllAsync());

            rol.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IRolRepository repository) =>
            {
                var r = await repository.GetByIdAsync(id);
                return r is not null ? Results.Ok(r) : Results.NotFound();
            });

            rol.MapPost("/", async (RolDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IRolRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var r = new Role
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion
                };

                await repository.AddAsync(r);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/rol/{r.RolId}", r);
            });

            rol.MapPut("/{id:int}", async (int id, RolDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IRolRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var existingRol = await repository.GetByIdAsync(id);
                if (existingRol is null) return Results.NotFound();
                
                existingRol.Nombre = dto.Nombre;
                existingRol.Descripcion = dto.Descripcion;
                
                repository.Update(existingRol);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            rol.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IRolRepository repository) =>
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


