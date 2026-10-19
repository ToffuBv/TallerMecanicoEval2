using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
using TallerMecanico.Repositories;

namespace TallerMecanico.Endpoints
{
    public static class VehiculoAPI
    {
        public static void MapVehiculoApi(this WebApplication app)
        {
            var vehiculo = app.MapVersionedV1Group("vehiculos", "Vehiculos");
            vehiculo.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IVehiculoRepository repository) => await repository.GetAllAsync());

            vehiculo.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IVehiculoRepository repository) =>
            {
                var v = await repository.GetByIdAsync(id);
                return v is not null ? Results.Ok(v) : Results.NotFound();
            });

            vehiculo.MapPost("/", async (VehiculoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IVehiculoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var v = new Vehiculo
                {
                    Patente = dto.Patente,
                    Anio = dto.Anio,
                    Color = dto.Color,
                    ClienteId = dto.ClienteId,
                    ModeloId = dto.ModeloId
                };

                await repository.AddAsync(v);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/vehiculos/{v.VehiculoId}", v);
            });

            vehiculo.MapPut("/{id:int}", async (int id, VehiculoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IVehiculoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var v = await repository.GetByIdAsync(id);
                if (v is null) return Results.NotFound();
                
                v.Patente = dto.Patente;
                v.Anio = dto.Anio;
                v.Color = dto.Color;
                v.ClienteId = dto.ClienteId;
                v.ModeloId = dto.ModeloId;
                
                repository.Update(v);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            vehiculo.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IVehiculoRepository repository) =>
            {
                var v = await repository.GetByIdAsync(id);
                if (v is null) return Results.NotFound();
                repository.Remove(v);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}


