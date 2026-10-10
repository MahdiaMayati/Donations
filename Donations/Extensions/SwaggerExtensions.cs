using Microsoft.OpenApi.Models;

namespace Donations.Extensions;

public static class SwaggerExtensions
{
    /// <summary>OpenAPI document names used by Swagger UI dropdown and controller GroupName.</summary>
    public static class Docs
    {
        public const string Security = "security";
        public const string Tasks = "tasks";
        public const string Donations = "donations";
        public const string Warehouses = "warehouses";
        public const string Volunteers = "volunteers";
        public const string Catalog = "catalog";
        public const string Locations = "locations";
        public const string Organizations = "organizations";
        public const string Beneficiaries = "beneficiaries";
        public const string System = "system";
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(Docs.Security, new OpenApiInfo
            {
                Title = "Donation API — Security",
                Version = "v1",
                Description = "Authentication, users, roles and permissions."
            });
            c.SwaggerDoc(Docs.Tasks, new OpenApiInfo
            {
                Title = "Donation API — Tasks",
                Version = "v1",
                Description = "Task types and related task endpoints."
            });
            c.SwaggerDoc(Docs.Donations, new OpenApiInfo
            {
                Title = "Donation API — Donations",
                Version = "v1",
                Description = "Donation requests, donors, and items."
            });
            c.SwaggerDoc(Docs.Warehouses, new OpenApiInfo
            {
                Title = "Donation API — Warehouses",
                Version = "v1",
                Description = "Warehouses and storage locations."
            });
            c.SwaggerDoc(Docs.Volunteers, new OpenApiInfo
            {
                Title = "Donation API — Volunteers",
                Version = "v1",
                Description = "Volunteer profiles and status."
            });
            c.SwaggerDoc(Docs.Catalog, new OpenApiInfo
            {
                Title = "Donation API — Catalog",
                Version = "v1",
                Description = "Item categories, types, colors, and materials."
            });
            c.SwaggerDoc(Docs.Locations, new OpenApiInfo
            {
                Title = "Donation API — Locations",
                Version = "v1",
                Description = "Cities, areas, and addresses."
            });
            c.SwaggerDoc(Docs.Organizations, new OpenApiInfo
            {
                Title = "Donation API — Organizations",
                Version = "v1",
                Description = "Organizations."
            });
            c.SwaggerDoc(Docs.Beneficiaries, new OpenApiInfo
            {
                Title = "Donation API — Beneficiaries",
                Version = "v1",
                Description = "Beneficiaries and family members."
            });
            c.SwaggerDoc(Docs.System, new OpenApiInfo
            {
                Title = "Donation API — System",
                Version = "v1",
                Description = "System settings, statistics, and misc."
            });

            c.DocInclusionPredicate((docName, apiDesc) =>
                string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase));

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "Paste ONLY the access token from /api/Auth/login (the value of \"token\").\n" +
                    "Do NOT paste refreshToken.\n" +
                    "Do NOT type the word Bearer — Swagger adds it.\n" +
                    "Do NOT include quotes."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{Docs.Security}/swagger.json", "Security");
            c.SwaggerEndpoint($"/swagger/{Docs.Tasks}/swagger.json", "Tasks");
            c.SwaggerEndpoint($"/swagger/{Docs.Donations}/swagger.json", "Donations");
            c.SwaggerEndpoint($"/swagger/{Docs.Warehouses}/swagger.json", "Warehouses");
            c.SwaggerEndpoint($"/swagger/{Docs.Volunteers}/swagger.json", "Volunteers");
            c.SwaggerEndpoint($"/swagger/{Docs.Catalog}/swagger.json", "Catalog");
            c.SwaggerEndpoint($"/swagger/{Docs.Locations}/swagger.json", "Locations");
            c.SwaggerEndpoint($"/swagger/{Docs.Organizations}/swagger.json", "Organizations");
            c.SwaggerEndpoint($"/swagger/{Docs.Beneficiaries}/swagger.json", "Beneficiaries");
            c.SwaggerEndpoint($"/swagger/{Docs.System}/swagger.json", "System");
        });
        return app;
    }
}
