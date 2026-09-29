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
                    CreatedAt = DateTime.UtcNow,
                    IsPreset = true,
                    IsDeleted = false
                };

                var userRole = new Role
                {
                    Name = "User",
                    NormalizedName = "USER",
                    RoleLevel = 2,
                    Description = "Standard user role",
                    CreatedAt = DateTime.UtcNow,
                    IsPreset = true,
                    IsDeleted = false
                };

                await roleManager.CreateAsync(adminRole);
                await roleManager.CreateAsync(userRole);
                logger.LogInformation("Seeded default roles successfully.");
            }
            else
            {
                // Backfill IsPreset on existing built-in roles after schema change.
                var systemRoles = await roleManager.Roles
                    .Where(r => (r.Name == "Admin" || r.Name == "User") && r.IsPreset == null)
                    .ToListAsync();

                foreach (var role in systemRoles)
                {
                    role.IsPreset = true;
                    await roleManager.UpdateAsync(role);
                }
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

            await SeedPermissionsAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static async Task SeedPermissionsAsync(AppDbContext context, ILogger logger)
    {
        var existingCodes = await context.Permissions
            .Select(p => p.Code)
            .ToListAsync();

        var existingSet = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var toAdd = new List<Permission>();

        foreach (var code in Donation.Application.Constants.Permissions.AllPermissionsList)
        {
            if (existingSet.Contains(code))
            {
                continue;
            }

            toAdd.Add(new Permission
            {
                Code = code,
                Description = code.Replace("Permissions.", string.Empty).Replace('.', ' ')
            });
        }

        if (toAdd.Count == 0)
        {
            return;
        }

        context.Permissions.AddRange(toAdd);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} permissions successfully.", toAdd.Count);
    }
}