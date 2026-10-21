using TallerMecanico.Models;
using TallerMecanico.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TallerMecanico.ValidacionesDTO;
using Microsoft.AspNetCore.Authorization;
using TallerMecanico.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TallerMecanico.Endpoints
{
    public static class UsuarioApi
    {
        public static void MapUsuarioApi(this WebApplication app)
        {
            var user = app.MapVersionedV1Group("usuarios", "Usuarios");

            user.MapGet("/", async (TallerMecanicoDbContext db) =>
                Results.Ok(await db.Usuarios.AsNoTracking().Select(u => new { u.UsuarioId, u.Nombre, u.Correo, u.RolId, Rol = u.Rol.Nombre }).ToListAsync()))
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            user.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var usuario = await db.Usuarios.AsNoTracking().Where(u => u.UsuarioId == id)
                    .Select(u => new { u.UsuarioId, u.Nombre, u.Correo, u.RolId, Rol = u.Rol.Nombre }).SingleOrDefaultAsync();
                return usuario is null ? Results.NotFound() : Results.Ok(usuario);
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            user.MapPost("/", async (AdministrarUsuarioDTO dto, TallerMecanicoDbContext db, AuthService auth) =>
            {
                if (dto.Validar() is { } validation) return validation;
                if (string.IsNullOrWhiteSpace(dto.Password)) return Results.ValidationProblem(new Dictionary<string, string[]> { ["Password"] = ["La contraseña es obligatoria al crear un usuario."] });
                if (await ValidarUsuario(dto, db) is { } error) return error;
                var usuario = new Usuario { Nombre = dto.Nombre.Trim(), Correo = dto.Correo.Trim(), RolId = dto.RolId };
                usuario.PasswordHash = auth.HashPassword(usuario, dto.Password);
                db.Usuarios.Add(usuario);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/usuarios/{usuario.UsuarioId}", new { usuario.UsuarioId, usuario.Nombre, usuario.Correo, usuario.RolId });
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            user.MapPut("/{id:int}", async (int id, AdministrarUsuarioDTO dto, TallerMecanicoDbContext db, AuthService auth) =>
            {
                if (dto.Validar() is { } validation) return validation;
                var usuario = await db.Usuarios.Include(u => u.Rol).SingleOrDefaultAsync(u => u.UsuarioId == id);
                if (usuario is null) return Results.NotFound();
                if (await ValidarUsuario(dto, db, id) is { } error) return error;
                var nextRole = await db.Roles.FindAsync(dto.RolId);
                if (usuario.Rol.Nombre == "Admin" && nextRole!.Nombre != "Admin" && !await db.Usuarios.AnyAsync(u => u.UsuarioId != id && u.Rol.Nombre == "Admin"))
                    return Results.Conflict(new { message = "No se puede cambiar el rol del último administrador." });
                usuario.Nombre = dto.Nombre.Trim(); usuario.Correo = dto.Correo.Trim(); usuario.RolId = dto.RolId;
                if (!string.IsNullOrEmpty(dto.Password)) usuario.PasswordHash = auth.HashPassword(usuario, dto.Password);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            user.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db, ClaimsPrincipal current) =>
            {
                var usuario = await db.Usuarios.Include(u => u.Rol).SingleOrDefaultAsync(u => u.UsuarioId == id);
                if (usuario is null) return Results.NotFound();
                if (current.Identity?.Name == usuario.Nombre)
                    return Results.Conflict(new { message = "No puedes eliminar tu propia cuenta." });
                if (usuario.Rol.Nombre == "Admin" && !await db.Usuarios.AnyAsync(u => u.UsuarioId != id && u.Rol.Nombre == "Admin"))
                    return Results.Conflict(new { message = "No se puede eliminar el último administrador." });
                db.Usuarios.Remove(usuario); await db.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

            user.MapPost("/registro", async (RegistroRequest request, [Microsoft.AspNetCore.Mvc.FromServices] IUsuarioRepository usuarioRepo, [Microsoft.AspNetCore.Mvc.FromServices] IRolRepository rolRepo, AuthService auth) => {
                    if (request.Validar() is { } errorDeValidacion) return errorDeValidacion;

                    var existentes = await usuarioRepo.FindAsync(u => u.Nombre == request.Nombre || u.Correo == request.Correo);
                    if (existentes.Any()) return Results.ValidationProblem(new Dictionary<string, string[]>
                    {
                        ["Nombre"] = new[] { "El usuario o correo ya está registrado." }
                    });

                    var roles = await rolRepo.FindAsync(rol => rol.Nombre == "Mecanico");
                    var rolMecanico = roles.SingleOrDefault();
                    if (rolMecanico is null)
                    {
                        rolMecanico = new Role { Nombre = "Mecanico" };
                        await rolRepo.AddAsync(rolMecanico);
                        await rolRepo.SaveChangesAsync();
                    }

                    var usuario = new Usuario
                    {
                        Nombre = request.Nombre,
                        Correo = request.Correo,
                        RolId = rolMecanico.RolId
                    };

                    usuario.PasswordHash = auth.HashPassword(usuario, request.Password);
                    await usuarioRepo.AddAsync(usuario);
                    await usuarioRepo.SaveChangesAsync();
                    
                    return Results.Created($"/api/v1/usuarios/{usuario.UsuarioId}", new { usuario.UsuarioId, usuario.Nombre, usuario.Correo });
                }).AllowAnonymous();

            user.MapPost("/login", async (LoginRequest login, [Microsoft.AspNetCore.Mvc.FromServices] IUsuarioRepository usuarioRepo,
                IConfiguration config, IWebHostEnvironment environment) =>
            {
                if (login.Validar() is { } errorDeValidacion) return errorDeValidacion;
                var usuario = await usuarioRepo.GetUsuarioConRolByNombreAsync(login.NombreUsuario);

                if (usuario is null)
                    return Results.Unauthorized();

                var hasher = new PasswordHasher<Usuario>();
                var resultado = hasher.VerifyHashedPassword(
                    usuario, usuario.PasswordHash, login.Password);

                if (resultado == PasswordVerificationResult.Failed)
                    return Results.Unauthorized();

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Role, usuario.Rol.Nombre)

                };
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                var jwtAudience = config["Jwt:Audience"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    jwtKey ?? throw new InvalidOperationException("Jwt:Key is missing.")));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    // Sesión ampliada temporalmente para las pruebas locales.
                    expires: DateTime.UtcNow.AddMinutes(environment.IsDevelopment() ? 60 : 5),
                    signingCredentials: credenciales
                );

                return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }).AllowAnonymous().RequireRateLimiting("login");
        }
        public record LoginRequest(string NombreUsuario, string Password);
        private static async Task<IResult?> ValidarUsuario(AdministrarUsuarioDTO dto, TallerMecanicoDbContext db, int? id = null)
        {
            if (!await db.Roles.AnyAsync(r => r.RolId == dto.RolId && (r.Nombre == "Admin" || r.Nombre == "Mecanico")))
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["RolId"] = ["Selecciona Admin o Mecanico."] });
            var nombre = dto.Nombre.Trim(); var correo = dto.Correo.Trim();
            if (await db.Usuarios.AnyAsync(u => u.UsuarioId != id && (u.Nombre == nombre || u.Correo == correo)))
                return Results.ValidationProblem(new Dictionary<string, string[]> { ["Nombre"] = ["El usuario o correo ya está registrado."] });
            return null;
        }
    }
}


