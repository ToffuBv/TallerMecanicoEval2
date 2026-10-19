using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Services;

public static class DevelopmentSeed
{
    public static async Task RunAsync(TallerMecanicoDbContext db, IConfiguration config, AuthService auth)
    {
        var adminPassword = config["Seed:AdminPassword"];
        var mechanicPassword = config["Seed:MecanicoPassword"];
        if (string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrWhiteSpace(mechanicPassword)) return;
        if (adminPassword.Length < 12 || mechanicPassword.Length < 12)
            throw new InvalidOperationException("Las contraseñas de desarrollo deben tener al menos 12 caracteres.");
        foreach (var (name, roleName, password) in new[] { ("admin", "Admin", adminPassword), ("mecanico", "Mecanico", mechanicPassword) })
        {
            var role = await db.Roles.FirstOrDefaultAsync(r => r.Nombre == roleName);
            if (role is null) { role = new Role { Nombre = roleName }; db.Roles.Add(role); await db.SaveChangesAsync(); }
            if (!await db.Usuarios.AnyAsync(u => u.Nombre == name))
            {
                var user = new Usuario { Nombre = name, Correo = $"{name}@taller.example", RolId = role.RolId };
                user.PasswordHash = auth.HashPassword(user, password); db.Usuarios.Add(user);
            }
        }
        if (!await db.Clientes.AnyAsync())
        {
            var client = new Cliente { Nombre = "Ana", Apellido = "Pérez" };
            var model = new Modelo { Nombre = "Corolla", Marca = new Marca { Nombre = "Toyota" } };
            db.Vehiculos.Add(new Vehiculo { Patente = "ABCD12", Anio = 2020, Cliente = client, Modelo = model });
        }
        if (!await db.Mecanicos.AnyAsync())
            db.Mecanicos.Add(new Mecanico { Nombre = "Luis", Apellido = "Soto", Especialidad = new Especialidade { Nombre = "Mecánica general" } });
        await db.SaveChangesAsync();
    }
}
