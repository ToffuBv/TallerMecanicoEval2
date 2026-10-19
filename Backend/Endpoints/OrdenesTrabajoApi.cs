using Microsoft.AspNetCore.Authorization;
using TallerMecanico.Models;
using TallerMecanico.Repositories;
using TallerMecanico.ValidacionesDTO;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace TallerMecanico.Endpoints
{
    public static class OrdenesTrabajoApi
    {
        public static void MapOrdenesTrabajoApi(this WebApplication app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var ordenesTrabajo = app.MapGroup("/api/v{version:apiVersion}/ordenes-trabajo")
                .WithApiVersionSet(apiVersionSet)
                .HasApiVersion(new ApiVersion(1, 0))
                .WithTags("OrdenesTrabajo");

            // Requiere autenticación y rol Admin o Mecanico
            ordenesTrabajo.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Mecanico" });

            ordenesTrabajo.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IOrdenesTrabajoRepository repository) =>
            {
                var ordenes = await repository.GetOrdenesConDetallesAsync();
                return Results.Ok(ordenes);
            });

            ordenesTrabajo.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IOrdenesTrabajoRepository repository) =>
            {
                var orden = await repository.GetOrdenConDetallesByIdAsync(id);
                return orden is not null ? Results.Ok(orden) : Results.NotFound();
            });

            ordenesTrabajo.MapPost("/", async (OrdenTrabajoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IOrdenesTrabajoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var orden = new OrdenesTrabajo
                {
                    FechaIngreso = dto.FechaIngreso,
                    Estado = dto.Estado,
                    Observaciones = dto.Observaciones,
                    ClienteId = dto.ClienteId,
                    VehiculoId = dto.VehiculoId,
                    MecanicoId = dto.MecanicoId
                };

                await repository.AddAsync(orden);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/ordenes-trabajo/{orden.OrdenId}", orden);
            });

            ordenesTrabajo.MapPut("/{id:int}", async (int id, OrdenTrabajoDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IOrdenesTrabajoRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var orden = await repository.GetByIdAsync(id);
                if (orden is null) return Results.NotFound();
                
                orden.Observaciones = dto.Observaciones;
                orden.FechaIngreso = dto.FechaIngreso;
                orden.Estado = dto.Estado;
                orden.ClienteId = dto.ClienteId;
                orden.VehiculoId = dto.VehiculoId;
                orden.MecanicoId = dto.MecanicoId;

                repository.Update(orden);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            // Solo Admin puede eliminar
            ordenesTrabajo.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IOrdenesTrabajoRepository repository) =>
            {
                var orden = await repository.GetByIdAsync(id);
                if (orden is null) return Results.NotFound();
                
                repository.Remove(orden);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        }
    }
}


