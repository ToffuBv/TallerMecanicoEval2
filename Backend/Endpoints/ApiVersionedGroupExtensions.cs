using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Authorization;

namespace TallerMecanico.Endpoints
{
    public static class ApiVersionedGroupExtensions
    {
        public static RouteGroupBuilder MapVersionedV1Group(this WebApplication app, string route, string tag, string roles = "Admin, Mecanico")
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            return app.MapGroup($"/api/v{{version:apiVersion}}/{route}")
                .WithApiVersionSet(apiVersionSet)
                .HasApiVersion(new ApiVersion(1, 0))
                .WithTags(tag)
                .RequireAuthorization(new AuthorizeAttribute { Roles = roles });
        }
    }
}


