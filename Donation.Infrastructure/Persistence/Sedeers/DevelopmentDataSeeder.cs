using Donation.Domain.Entities;
using Donation.Domain.Enums;
using Donation.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeds fictional data across domain tables for local Development testing.
/// Each section is idempotent (fills only when that section is empty).
/// </summary>
public static class DevelopmentDataSeeder
{
    public const string DemoDonorEmail = "donor@donation.com";
    public const string DemoDonorPassword = "Donor@12345";
    public const string DemoDonorDeletedEmail = "deleted.donor@donation.com";
    public const string DemoDonorDeletedPassword = "Donor@12345";
    public const string DemoVolunteerEmail = "volunteer@donation.com";
    public const string DemoVolunteerPassword = "Volunteer@12345";
    public const string DemoVolunteerPendingEmail = "volunteer.pending@donation.com";
    public const string DemoVolunteerPendingPassword = "Volunteer@12345";
    public const string DemoBeneficiaryEmail = "beneficiary@donation.com";
    public const string DemoBeneficiaryPassword = "Beneficiary@12345";

    public static async Task SeedAsync(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var organization = await SeedOrganizationsAsync(context, logger, cancellationToken);
        var areas = await SeedCitiesAndAreasAsync(context, logger, cancellationToken);
        await SeedProfilesAsync(userManager, context, organization, areas, logger, cancellationToken);
        await SeedAdminRolePermissionsAsync(roleManager, context, logger, cancellationToken);
        await SeedSampleRefreshTokenAsync(userManager, context, logger, cancellationToken);

        logger.LogInformation("Development data seeding finished.");
    }

    private static async Task<Organization> SeedOrganizationsAsync(
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var existing = await context.Organizations
            .FirstOrDefaultAsync(o => !o.IsDeleted, cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var active = new Organization
        {
            Name = "Hope Donation Center",
            IsActive = true,
            IsDeleted = false
        };

        var inactive = new Organization
        {
            Name = "Legacy Aid Org (Inactive)",
            IsActive = false,
            IsDeleted = false
        };

        context.Organizations.AddRange(active, inactive);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} organizations.", 2);
        return active;
    }

    private static async Task<IReadOnlyList<Area>> SeedCitiesAndAreasAsync(
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (!await context.Cities.AnyAsync(cancellationToken))
        {
            var cities = new[]
            {
                new City { Name = "Ramallah", Code = "RAM" },
                new City { Name = "Nablus", Code = "NAB" },
                new City { Name = "Hebron", Code = "HEB" },
                new City { Name = "Bethlehem", Code = "BTH" },
                new City { Name = "Jenin", Code = "JEN" }
            };

            context.Cities.AddRange(cities);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} cities.", cities.Length);
        }

        if (!await context.Areas.AnyAsync(cancellationToken))
        {
            var cities = await context.Cities.OrderBy(c => c.Name).ToListAsync(cancellationToken);
            var areas = new List<Area>();

            foreach (var city in cities)
            {
                areas.Add(new Area { CityId = city.Id, Name = $"{city.Name} Center" });
                areas.Add(new Area { CityId = city.Id, Name = $"{city.Name} North" });
            }

            context.Areas.AddRange(areas);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} areas.", areas.Count);
            return areas;
        }

        return await context.Areas.OrderBy(a => a.Name).ToListAsync(cancellationToken);
    }

    private static async Task SeedProfilesAsync(
        UserManager<User> userManager,
        AppDbContext context,
        Organization organization,
        IReadOnlyList<Area> areas,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (areas.Count == 0)
        {
            logger.LogWarning("No areas available; skipping profile seeding.");
            return;
        }

        var hasDonors = await context.Donors.IgnoreQueryFilters().AnyAsync(cancellationToken);
        if (hasDonors)
        {
            logger.LogInformation("Profile sample data already present; skipping profiles.");
            return;
        }

        var area0 = areas[0];
        var area1 = areas.Count > 1 ? areas[1] : areas[0];
        var area2 = areas.Count > 2 ? areas[2] : areas[0];

        var donorUser = await EnsureUserAsync(
            userManager, DemoDonorEmail, DemoDonorPassword,
            "Ahmad", "Donor", "WhatsApp", "User", organization.Id, logger);

        var deletedDonorUser = await EnsureUserAsync(
            userManager, DemoDonorDeletedEmail, DemoDonorDeletedPassword,
            "Khaled", "DeletedDonor", "SMS", "User", organization.Id, logger);

        var volunteerUser = await EnsureUserAsync(
            userManager, DemoVolunteerEmail, DemoVolunteerPassword,
            "Sara", "Volunteer", "Call", "User", organization.Id, logger);

        var pendingVolunteerUser = await EnsureUserAsync(
            userManager, DemoVolunteerPendingEmail, DemoVolunteerPendingPassword,
            "Maya", "PendingVolunteer", "WhatsApp", "User", null, logger);

        var beneficiaryUser = await EnsureUserAsync(
            userManager, DemoBeneficiaryEmail, DemoBeneficiaryPassword,
            "Lina", "Beneficiary", "SMS", "User", organization.Id, logger);

        var donorAddress = new Address
        {
            AreaId = area0.Id,
            UserId = donorUser.Id,
            Street = "Main Street 12",
            Details = "Near the municipality",
            Latitude = 31.9038,
            Longitude = 35.2034
        };

        var deletedDonorAddress = new Address
        {
            AreaId = area1.Id,
            UserId = deletedDonorUser.Id,
            Street = "Old Market 3",
            Details = "Soft-deleted donor sample address",
            Latitude = 32.2211,
            Longitude = 35.2544
        };

        var volunteerAddress = new Address
        {
            AreaId = area2.Id,
            UserId = volunteerUser.Id,
            Street = "Volunteer Lane 7",
            Details = "Apartment 4",
            Latitude = 31.7054,
            Longitude = 35.2024
        };

        var beneficiaryAddress = new Address
        {
            AreaId = area1.Id,
            UserId = beneficiaryUser.Id,
            Street = "School Street 5",
            Details = "Building B, floor 2",
            Latitude = 31.8990,
            Longitude = 35.2045
        };

        context.Addresses.AddRange(donorAddress, deletedDonorAddress, volunteerAddress, beneficiaryAddress);
        await context.SaveChangesAsync(cancellationToken);

        context.Donors.AddRange(
            new Donor
            {
                UserId = donorUser.Id,
                IsDeleted = false,
                DeletedAt = null
            },
            new Donor
            {
                UserId = deletedDonorUser.Id,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-2)
            });

        context.Volunteers.AddRange(
            new Volunteer
            {
                UserId = volunteerUser.Id,
                Status = VolunteerStatus.Active
            },
            new Volunteer
            {
                UserId = pendingVolunteerUser.Id,
                Status = VolunteerStatus.Pending
            });

        var beneficiary = new Beneficiary
        {
            UserId = beneficiaryUser.Id,
            AddressId = beneficiaryAddress.Id,
            IdPhotoUrl = "https://example.com/sample-id.jpg",
            IsHeadOfHousehold = true,
            VerificationStatus = VerificationStatus.Verified,
            VerifiedUntil = DateTime.UtcNow.AddMonths(6),
            IsDeleted = false
        };

        context.Beneficiaries.Add(beneficiary);
        await context.SaveChangesAsync(cancellationToken);

        context.FamilyMembers.AddRange(
            new FamilyMember
            {
                BeneficiaryId = beneficiary.Id,
                FullName = "Omar Beneficiary",
                BirthDate = new DateTime(2015, 4, 12),
                Gender = Gender.Male,
                ClothingSize = ClothingSize.M,
                ShoeSize = "36",
                IsDeleted = false
            },
            new FamilyMember
            {
                BeneficiaryId = beneficiary.Id,
                FullName = "Nour Beneficiary",
                BirthDate = new DateTime(2018, 9, 1),
                Gender = Gender.Female,
                ClothingSize = ClothingSize.S,
                ShoeSize = "32",
                IsDeleted = false
            },
            new FamilyMember
            {
                BeneficiaryId = beneficiary.Id,
                FullName = "Soft-Deleted Child",
                BirthDate = new DateTime(2012, 1, 20),
                Gender = Gender.Male,
                ClothingSize = ClothingSize.L,
                ShoeSize = "38",
                IsDeleted = true
            });

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Seeded profiles: addresses, donors (incl. soft-deleted), volunteers, beneficiary, family members.");
    }

    private static async Task SeedAdminRolePermissionsAsync(
        RoleManager<Role> roleManager,
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var adminRole = await roleManager.FindByNameAsync("Admin");
        if (adminRole is null)
        {
            logger.LogWarning("Admin role not found; skipping RolePermissions seed.");
            return;
        }

        var existingCount = await context.RolePermissions
            .CountAsync(rp => rp.RoleId == adminRole.Id, cancellationToken);

        if (existingCount > 0)
        {
            return;
        }

        var permissions = await context.Permissions.ToListAsync(cancellationToken);
        if (permissions.Count == 0)
        {
            logger.LogWarning("No permissions found; skipping RolePermissions seed.");
            return;
        }

        foreach (var permission in permissions)
        {
            context.RolePermissions.Add(RolePermission.Create(adminRole.Id, permission.Id));
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Assigned {Count} permissions to Admin role.", permissions.Count);
    }

    private static async Task SeedSampleRefreshTokenAsync(
        UserManager<User> userManager,
        AppDbContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        if (await context.RefreshTokens.AnyAsync(cancellationToken))
        {
            return;
        }

        var admin = await userManager.FindByEmailAsync("admin@donation.com");
        if (admin is null)
        {
            return;
        }

        context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
            UserId = admin.Id,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            RevokedOn = null
        });

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded sample refresh token for admin.");
    }

    private static async Task<User> EnsureUserAsync(
        UserManager<User> userManager,
        string email,
        string password,
        string firstName,
        string lastName,
        string preferredContact,
        string roleName,
        Guid? organizationId,
        ILogger logger)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            if (organizationId.HasValue && existing.OrganizationId != organizationId)
            {
                existing.OrganizationId = organizationId;
                await userManager.UpdateAsync(existing);
            }

            return existing;
        }

        var user = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FirstName = firstName,
            LastName = lastName,
            IsActive = true,
            PreferredContactMethod = preferredContact,
            PhoneNumber = "0599" + Math.Abs(email.GetHashCode() % 1000000).ToString("D6"),
            OrganizationId = organizationId,
            Gender = true,
            MaritalStatus = "Single",
            EducationalStatus = "Bachelor",
            Job = "Sample",
            HealthStatus = "Good",
            DateOfBirth = new DateTime(1995, 5, 15)
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to seed user {email}: {errors}");
        }

        await userManager.AddToRoleAsync(user, roleName);
        logger.LogInformation("Seeded demo user {Email}.", email);
        return user;
    }
}
