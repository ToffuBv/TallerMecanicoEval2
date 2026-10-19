using Microsoft.AspNetCore.Authorization;
using TallerMecanico.Models;
using TallerMecanico.Repositories;
using TallerMecanico.ValidacionesDTO;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace TallerMecanico.Endpoints
{
    public static class ClienteApi
    {
        public static void MapClienteApi(this WebApplication app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var clientes = app.MapGroup("/api/v{version:apiVersion}/clientes")
                .WithApiVersionSet(apiVersionSet)
                .HasApiVersion(new ApiVersion(1, 0))
                .WithTags("Clientes");

            // Requiere autenticación y rol Admin o Mecanico para acceder a clientes
            clientes.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Mecanico" });

            clientes.MapGet("/", async ([Microsoft.AspNetCore.Mvc.FromServices] IClienteRepository repository) =>
            {
                var clientesList = await repository.GetAllAsync();
                return Results.Ok(clientesList);
            });

            clientes.MapGet("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IClienteRepository repository) =>
            {
                var cliente = await repository.GetByIdAsync(id);
                return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
            });

            clientes.MapPost("/", async (ClienteDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IClienteRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var cliente = new Cliente
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Telefono = dto.Telefono,
                    Correo = dto.Correo,
                    Direccion = dto.Direccion
                };

                await repository.AddAsync(cliente);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/clientes/{cliente.ClienteId}", cliente);
            });

            clientes.MapPut("/{id:int}", async (int id, ClienteDTO dto, [Microsoft.AspNetCore.Mvc.FromServices] IClienteRepository repository) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var cliente = await repository.GetByIdAsync(id);
                if (cliente is null) return Results.NotFound();

                cliente.Nombre = dto.Nombre;
                cliente.Apellido = dto.Apellido;
                cliente.Telefono = dto.Telefono;
                cliente.Correo = dto.Correo;
                cliente.Direccion = dto.Direccion;

                repository.Update(cliente);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            // Solo el rol Admin puede eliminar clientes
            clientes.MapDelete("/{id:int}", async (int id, [Microsoft.AspNetCore.Mvc.FromServices] IClienteRepository repository) =>
            {
                var cliente = await repository.GetByIdAsync(id);
                if (cliente is null) return Results.NotFound();

                repository.Remove(cliente);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        }
    }
}


