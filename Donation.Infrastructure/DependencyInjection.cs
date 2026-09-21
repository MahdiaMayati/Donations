
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
        // 1. تسجيل قاعدة البيانات
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // 2. تسجيل الواجهات والخدمات الأساسية
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}