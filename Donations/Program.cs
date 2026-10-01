using Donation.Application;
using Donation.Domain.Entities;
using Donation.Infrastructure;
using Donation.Infrastructure.Persistence;
using Donation.Infrastructure.Persistence.Seeders;
using Donations.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// Prevent long ClaimTypes URI remapping (role/sub stay as short JWT names).
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddApiRateLimiting();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ==========================================
// Identity for APIs: AddIdentityCore (NOT AddIdentity).
// AddIdentity registers cookie auth and can fight JWT Bearer defaults.
// ==========================================
builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager();

// ==========================================
// JWT Bearer — single auth scheme for this API
// ==========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"]
    ?? throw new InvalidOperationException("JwtSettings:Secret is missing.");

if (Encoding.UTF8.GetByteCount(secretKey) < 32)
{
    throw new InvalidOperationException("JwtSettings:Secret must be at least 32 bytes for HS256.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.IncludeErrorDetails = true;
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; // local Swagger over https/http profiles

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = "role"
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Normalize Swagger / client paste mistakes before validation.
                var raw = context.Request.Headers.Authorization.ToString();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return Task.CompletedTask;
                }

                // StringValues.ToString() can join multiple values with ", " — take first only.
                var header = raw.Split(',', 2, StringSplitOptions.TrimEntries)[0];

                var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? header["Bearer ".Length..]
                    : header;

                token = token.Trim().Trim('"').Trim('\'');
                while (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token["Bearer ".Length..].Trim().Trim('"');
                }

                context.Token = token;
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtBearer");
                logger.LogWarning(context.Exception,
                    "JWT auth failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim(c =>
                (c.Type == "role" || c.Type == ClaimTypes.Role) && c.Value == "Admin")));

    foreach (var permission in Donation.Application.Constants.Permissions.AllPermissionsList)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireClaim("Permission", permission));
    }
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var logger = services.GetRequiredService<ILogger<RbacDbSeeder>>();
        await RbacDbSeeder.SeedAsync(userManager, roleManager, context, logger);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration/seeding.");
    }
}

app.UseSwaggerDocumentation();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Expose for WebApplicationFactory / integration tests.
public partial class Program;
