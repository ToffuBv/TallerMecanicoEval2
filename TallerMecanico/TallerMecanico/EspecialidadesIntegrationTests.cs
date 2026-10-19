using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using TallerMecanico.Services;
using TallerMecanico.Models;
using Xunit;

namespace TallerMecanico.IntegrationTests;

public sealed class EspecialidadesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EspecialidadesIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("Jwt:Key", "clave-exclusiva-de-pruebas-sin-uso-en-produccion-2026");
            builder.UseSetting("Jwt:Issuer", "TallerMecanicoApi");
            builder.UseSetting("Jwt:Audience", "TallerMecanicoApiUsuarios");
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<TallerMecanicoDbContext>>();
                services.RemoveAll<TallerMecanicoDbContext>();
                services.RemoveAll<IDbContextOptionsConfiguration<TallerMecanicoDbContext>>();
                services.AddDbContext<TallerMecanicoDbContext>(options =>
                    options.UseInMemoryDatabase("TallerMecanicoTests"));
            });
        });
    }

    [Fact]
    public async Task OpenApi_expone_el_contrato_de_especialidades()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var contract = await response.Content.ReadAsStringAsync();
        Assert.Contains("/api/v1/especialidad/", contract);
    }

    [Fact]
    public async Task CRUD_administrativo_rechaza_solicitudes_anonimas()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/especialidad/");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_expone_el_CRUD_de_ordenes_de_trabajo()
    {
        var client = _factory.CreateClient();
        using var document = JsonDocument.Parse(await client.GetStringAsync("/openapi/v1.json"));
        var paths = document.RootElement.GetProperty("paths");

        var collection = paths.GetProperty("/api/v1/ordenes-trabajo");
        Assert.True(collection.TryGetProperty("get", out _));
        Assert.True(collection.TryGetProperty("post", out _));

        var resource = paths.GetProperty("/api/v1/ordenes-trabajo/{id}");
        Assert.True(resource.TryGetProperty("get", out _));
        Assert.True(resource.TryGetProperty("put", out _));
        Assert.True(resource.TryGetProperty("delete", out _));
    }

    [Fact]
    public async Task Login_real_CRUD_y_roles_se_validan_en_el_servidor()
    {
        var client = _factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TallerMecanicoDbContext>();
        await DevelopmentSeed.RunAsync(db, new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Seed:AdminPassword"] = "solo-pruebas-admin-2026",
            ["Seed:MecanicoPassword"] = "solo-pruebas-mecanico-2026"
        }).Build(), scope.ServiceProvider.GetRequiredService<AuthService>());
        async Task SignIn(string name, string password)
        {
            var response = await client.PostAsJsonAsync("/api/v1/usuarios/login", new { nombreUsuario = name, password });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", json.RootElement.GetProperty("token").GetString());
        }
        await SignIn("mecanico", "solo-pruebas-mecanico-2026");
        Assert.Equal(HttpStatusCode.Forbidden, (await client.DeleteAsync("/api/v1/ordenes-trabajo/987654")).StatusCode);
        var cliente = await db.Clientes.FirstAsync();
        var vehiculo = await db.Vehiculos.FirstAsync();
        var mecanico = await db.Mecanicos.FirstAsync();
        var input = new { fechaIngreso = "2026-10-19", estado = "Pendiente", observaciones = "Prueba integrada", clienteId = cliente.ClienteId, vehiculoId = vehiculo.VehiculoId, mecanicoId = mecanico.MecanicoId };
        var created = await client.PostAsJsonAsync("/api/v1/ordenes-trabajo/", input);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        using var document = JsonDocument.Parse(await created.Content.ReadAsStringAsync());
        var id = document.RootElement.GetProperty("ordenId").GetInt32();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/v1/ordenes-trabajo/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/v1/ordenes-trabajo/{id}", input)).StatusCode);
        var invalid = await client.PostAsJsonAsync("/api/v1/ordenes-trabajo/", new { fechaIngreso = "2026-10-19", estado = "Inválido", observaciones = "", clienteId = 0, vehiculoId = 0, mecanicoId = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        await SignIn("admin", "solo-pruebas-admin-2026");
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/v1/ordenes-trabajo/{id}")).StatusCode);
    }

    [Fact]
    public async Task CORS_rechaza_origen_ajeno_y_cabeceras_estan_presentes()
    {
        var client = _factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/ordenes-trabajo/");
        request.Headers.Add("Origin", "https://sitio-ajeno.example");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        var response = await client.SendAsync(request);
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
    }

    [Fact]
    public async Task Login_limita_intentos_por_IP()
    {
        var client = _factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        HttpResponseMessage? response = null;
        for (var i = 0; i < 6; i++)
            response = await client.PostAsJsonAsync("/api/v1/usuarios/login", new { nombreUsuario = "inexistente", password = "invalida" });
        Assert.Equal(HttpStatusCode.TooManyRequests, response!.StatusCode);
    }
}
