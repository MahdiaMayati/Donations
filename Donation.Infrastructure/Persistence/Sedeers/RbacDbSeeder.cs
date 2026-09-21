using Donation.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Donation.Infrastructure.Persistence;

namespace Donation.Infrastructure.Persistence.Seeders;

public class RbacDbSeeder
{
    public static async Task SeedAsync(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        AppDbContext context,
        ILogger<RbacDbSeeder> logger)
    {
        try
        {
            await context.Database.MigrateAsync();

            if (!await roleManager.Roles.AnyAsync())
            {
                var adminRole = new Role
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    RoleLevel = 1,
                    Description = "Administrator role with full permissions",
                    CreatedAt = DateTime.UtcNow
                };

                var userRole = new Role
                {
                    Name = "User",
                    NormalizedName = "USER",
                    RoleLevel = 2,
                    Description = "Standard user role",
                    CreatedAt = DateTime.UtcNow
                };

                await roleManager.CreateAsync(adminRole);
                await roleManager.CreateAsync(userRole);
                logger.LogInformation("Seeded default roles successfully.");
            }

            var adminEmail = "admin@donation.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@12345");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Seeded default admin user successfully.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}