using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
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
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<TallerMecanicoDbContext>>();
                services.RemoveAll<TallerMecanicoDbContext>();
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
}