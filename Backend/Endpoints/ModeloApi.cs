using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class ModeloApi
    {
        public static void MapModeloApi(this WebApplication app)
        {
            var grupo = app.MapVersionedV1Group("modelo", "Modelo");
            
            grupo.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IModeloRepository repository) =>
            {
                var modelos = await repository.GetAllAsync();
                return Results.Ok(modelos);
            });
            
            grupo.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IModeloRepository repository) =>
            {
                var modelo = await repository.GetByIdAsync(id);
                return modelo is not null ? Results.Ok(modelo) : Results.NotFound();
            });
            
            grupo.MapPost("/", async (ModeloDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IModeloRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var modelo = new Modelo
                {
                    Nombre = dto.Nombre,
                    MarcaId = dto.MarcaId
                };

                await repository.AddAsync(modelo);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/modelo/{modelo.ModeloId}", modelo);
            });
            
            grupo.MapPut("/{id:int}", async (int id, ModeloDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IModeloRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var modelo = await repository.GetByIdAsync(id);
                if (modelo is null) return Results.NotFound();
                
                modelo.Nombre = dto.Nombre;
                modelo.MarcaId = dto.MarcaId;
                
                repository.Update(modelo);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
            
            grupo.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IModeloRepository repository) =>
            {
                var modelo = await repository.GetByIdAsync(id);
                if (modelo is null) return Results.NotFound();
                repository.Remove(modelo);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


