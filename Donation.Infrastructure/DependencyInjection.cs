using Donation.Application.Abstractions.Authentication;
using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Domain.Entities;
using Donation.Infrastructure.Authentication;
using Donation.Infrastructure.Persistence;
using Donation.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Donation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 1. Database + factory (factory enables parallel aggregation queries)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDbContextFactory<AppDbContext>(options =>
            options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

        services.AddMemoryCache();

        // 2. Core abstractions / services
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAboutUsStatisticsService, AboutUsStatisticsService>();
        services.AddScoped<IUserIdentityService, UserIdentityService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
