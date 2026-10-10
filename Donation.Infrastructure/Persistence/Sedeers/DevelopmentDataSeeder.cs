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
    public const string DemoBeneficiary2Email = "beneficiary2@donation.com";
    public const string DemoBeneficiary2Password = "Beneficiary@12345";

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
        await SeedFamilyMembersDemoAsync(userManager, context, organization, areas, logger, cancellationToken);
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

        await SeedDonorsAndHouseholdAsync(userManager, context, organization, areas, logger, cancellationToken);
        await SeedVolunteersAsync(userManager, context, organization, areas, logger, cancellationToken);
    }

    private static async Task SeedDonorsAndHouseholdAsync(
        UserManager<User> userManager,
        AppDbContext context,
        Organization organization,
        IReadOnlyList<Area> areas,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var hasDonors = await context.Donors.IgnoreQueryFilters().AnyAsync(cancellationToken);
        if (hasDonors)
        {
            logger.LogInformation("Donor/beneficiary sample data already present; skipping that section.");
            return;
        }

        var area0 = areas[0];
        var area1 = areas.Count > 1 ? areas[1] : areas[0];

        var donorUser = await EnsureUserAsync(
            userManager, DemoDonorEmail, DemoDonorPassword,
            "Ahmad", "Donor", "WhatsApp", "User", organization.Id, logger);

        var deletedDonorUser = await EnsureUserAsync(
            userManager, DemoDonorDeletedEmail, DemoDonorDeletedPassword,
            "Khaled", "DeletedDonor", "SMS", "User", organization.Id, logger);

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

        var beneficiaryAddress = new Address
        {
            AreaId = area1.Id,
            UserId = beneficiaryUser.Id,
            Street = "School Street 5",
            Details = "Building B, floor 2",
            Latitude = 31.8990,
            Longitude = 35.2045
        };

        context.Addresses.AddRange(donorAddress, deletedDonorAddress, beneficiaryAddress);
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

<<<<<<< HEAD
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
        logger.LogInformation("Seeded donor/beneficiary sample profiles.");
    }

    private static async Task SeedVolunteersAsync(
=======
        logger.LogInformation(
            "Seeded profiles: addresses, donors (incl. soft-deleted), volunteers, beneficiary.");
    }

    /// <summary>
    /// Idempotent Family Members demo data for exercising single + batch CRUD.
    /// Runs even when donors/profiles already exist.
    /// </summary>
    private static async Task SeedFamilyMembersDemoAsync(
>>>>>>> a211c529ed7431505e5bcb3244d053a964071773
        UserManager<User> userManager,
        AppDbContext context,
        Organization organization,
        IReadOnlyList<Area> areas,
        ILogger logger,
        CancellationToken cancellationToken)
    {
<<<<<<< HEAD
        var area0 = areas[0];
        var area2 = areas.Count > 2 ? areas[2] : areas[0];

        var volunteerUser = await EnsureUserAsync(
            userManager, DemoVolunteerEmail, DemoVolunteerPassword,
            "Sara", "Volunteer", "Call", "User", organization.Id, logger);

        var pendingVolunteerUser = await EnsureUserAsync(
            userManager, DemoVolunteerPendingEmail, DemoVolunteerPendingPassword,
            "Maya", "PendingVolunteer", "WhatsApp", "User", organization.Id, logger);

        // Enrich incomplete volunteers left over from schema migration.
        var incomplete = await context.Volunteers
            .IgnoreQueryFilters()
            .Where(v => v.Days == null || v.Days == string.Empty)
            .ToListAsync(cancellationToken);

        foreach (var volunteer in incomplete)
        {
            volunteer.OrganizationId = organization.Id;
            volunteer.Days = volunteer.UserId == volunteerUser.Id ? "Saturday,Sunday" : "Monday,Wednesday";
            volunteer.HoursCount = volunteer.UserId == volunteerUser.Id ? 8 : 4;
            volunteer.Hobbies = volunteer.UserId == volunteerUser.Id ? "Reading, Hiking" : "Photography";
            volunteer.Skills = volunteer.UserId == volunteerUser.Id ? "First aid, Logistics" : "Communication";
            if (string.IsNullOrWhiteSpace(volunteer.Status))
            {
                volunteer.Status = "Active";
            }
        }

        if (incomplete.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Enriched {Count} incomplete volunteer row(s).", incomplete.Count);
        }

        await EnsureAddressAsync(
            context,
            volunteerUser.Id,
            area2.Id,
            "Volunteer Lane 7",
            "Apartment 4",
            31.7054,
            35.2024,
            cancellationToken);

        await EnsureAddressAsync(
            context,
            pendingVolunteerUser.Id,
            area0.Id,
            "Pending Ave 1",
            "Floor 1",
            31.9000,
            35.2000,
            cancellationToken);

        var hasActiveVolunteer = await context.Volunteers
            .AnyAsync(v => v.UserId == volunteerUser.Id, cancellationToken);

        if (!hasActiveVolunteer)
        {
            context.Volunteers.Add(new Volunteer
            {
                UserId = volunteerUser.Id,
                OrganizationId = organization.Id,
                Status = "Active",
                Days = "Saturday,Sunday",
                HoursCount = 8,
                Hobbies = "Reading, Hiking",
                Skills = "First aid, Logistics",
                IsDeleted = false
            });
        }

        var hasPendingVolunteer = await context.Volunteers
            .IgnoreQueryFilters()
            .AnyAsync(v => v.UserId == pendingVolunteerUser.Id, cancellationToken);

        if (!hasPendingVolunteer)
        {
            context.Volunteers.Add(new Volunteer
            {
                UserId = pendingVolunteerUser.Id,
                OrganizationId = organization.Id,
                Status = "Pending",
                Days = "Monday,Wednesday",
                HoursCount = 4,
                Hobbies = "Photography",
                Skills = "Communication",
                IsDeleted = false
            });
        }

        // Soft-deleted sample for restore/deleted endpoints.
        var softDeletedEmail = "volunteer.deleted@donation.com";
        var softDeletedUser = await EnsureUserAsync(
            userManager, softDeletedEmail, DemoVolunteerPassword,
            "Omar", "DeletedVolunteer", "SMS", "User", organization.Id, logger);

        await EnsureAddressAsync(
            context,
            softDeletedUser.Id,
            area0.Id,
            "Archive Street 9",
            "Soft-deleted volunteer address",
            31.9100,
            35.2100,
            cancellationToken);

        var hasSoftDeleted = await context.Volunteers
            .IgnoreQueryFilters()
            .AnyAsync(v => v.UserId == softDeletedUser.Id, cancellationToken);

        if (!hasSoftDeleted)
        {
            context.Volunteers.Add(new Volunteer
            {
                UserId = softDeletedUser.Id,
                OrganizationId = organization.Id,
                Status = "Inactive",
                Days = "Friday",
                HoursCount = 2,
                Hobbies = "Chess",
                Skills = "Driving",
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Volunteer sample data ensured (active, pending, soft-deleted).");
    }

    private static async Task EnsureAddressAsync(
        AppDbContext context,
        Guid userId,
        Guid areaId,
        string street,
        string details,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        var exists = await context.Addresses.AnyAsync(a => a.UserId == userId, cancellationToken);
        if (exists)
        {
            return;
        }

        context.Addresses.Add(new Address
        {
            AreaId = areaId,
            UserId = userId,
            Street = street,
            Details = details,
            Latitude = latitude,
            Longitude = longitude
        });
        await context.SaveChangesAsync(cancellationToken);
=======
        if (areas.Count == 0)
        {
            logger.LogWarning("No areas available; skipping family members demo seed.");
            return;
        }

        var area0 = areas[0];
        var area1 = areas.Count > 1 ? areas[1] : areas[0];

        var beneficiaryUser = await EnsureUserAsync(
            userManager, DemoBeneficiaryEmail, DemoBeneficiaryPassword,
            "Lina", "Beneficiary", "SMS", "User", organization.Id, logger);

        var beneficiaryUser2 = await EnsureUserAsync(
            userManager, DemoBeneficiary2Email, DemoBeneficiary2Password,
            "Rami", "HeadTwo", "WhatsApp", "User", organization.Id, logger);

        await EnrichBeneficiaryUserProfileAsync(userManager, beneficiaryUser, logger);
        await EnrichBeneficiaryUserProfileAsync(userManager, beneficiaryUser2, logger);

        var beneficiary1 = await EnsureBeneficiaryAsync(
            context,
            beneficiaryUser,
            area1.Id,
            "https://example.com/sample-id-lina.jpg",
            isHeadOfHousehold: true,
            cancellationToken);

        var beneficiary2 = await EnsureBeneficiaryAsync(
            context,
            beneficiaryUser2,
            area0.Id,
            "https://example.com/sample-id-rami.jpg",
            isHeadOfHousehold: true,
            cancellationToken);

        var existingNames = await context.FamilyMembers
            .IgnoreQueryFilters()
            .Select(m => m.FullName)
            .ToListAsync(cancellationToken);

        var existingNameSet = existingNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = new List<FamilyMember>();

        void AddIfMissing(Guid beneficiaryId, string fullName, DateTime birthDate, bool gender, string clothingSize, string shoe, bool deleted)
        {
            if (existingNameSet.Contains(fullName))
            {
                return;
            }

            toAdd.Add(new FamilyMember
            {
                BeneficiaryId = beneficiaryId,
                FullName = fullName,
                BirthDate = birthDate,
                Gender = gender,
                ClothingSize = clothingSize,
                ShoeSize = shoe,
                IsDeleted = deleted
            });
        }

        // Household 1 — Lina (Gender: true=Male, false=Female)
        AddIfMissing(beneficiary1.Id, "Omar Beneficiary", new DateTime(2015, 4, 12), gender: true, clothingSize: "M", "36", false);
        AddIfMissing(beneficiary1.Id, "Nour Beneficiary", new DateTime(2018, 9, 1), gender: false, clothingSize: "S", "32", false);
        AddIfMissing(beneficiary1.Id, "Yara Beneficiary", new DateTime(2020, 2, 28), gender: false, clothingSize: "XS", "28", false);
        AddIfMissing(beneficiary1.Id, "Soft-Deleted Child", new DateTime(2012, 1, 20), gender: true, clothingSize: "L", "38", true);

        // Household 2 — Rami
        AddIfMissing(beneficiary2.Id, "Tariq HeadTwo", new DateTime(2014, 7, 8), gender: true, clothingSize: "L", "37", false);
        AddIfMissing(beneficiary2.Id, "Hala HeadTwo", new DateTime(2016, 11, 15), gender: false, clothingSize: "M", "34", false);

        if (toAdd.Count > 0)
        {
            context.FamilyMembers.AddRange(toAdd);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Seeded {Count} additional family member demo row(s).", toAdd.Count);
        }
        else
        {
            logger.LogInformation("Family member demo pack already complete; no inserts.");
        }

        var seededIds = await context.FamilyMembers
            .AsNoTracking()
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.FullName)
            .Select(m => new { m.Id, m.FullName, m.BeneficiaryId })
            .ToListAsync(cancellationToken);

        logger.LogInformation(
            "FamilyMembers test logins — {Email1} / {Password1} (HeadOfHouseholdId={HoH1}); {Email2} / {Password2} (HeadOfHouseholdId={HoH2})",
            DemoBeneficiaryEmail,
            DemoBeneficiaryPassword,
            beneficiaryUser.Id,
            DemoBeneficiary2Email,
            DemoBeneficiary2Password,
            beneficiaryUser2.Id);
        logger.LogInformation(
            "Admin can list all. Active seeded members: {Members}",
            string.Join("; ", seededIds.Select(m => $"{m.FullName}={m.Id}")));
    }

    private static async Task EnrichBeneficiaryUserProfileAsync(
        UserManager<User> userManager,
        User user,
        ILogger logger)
    {
        var changed = false;

        if (string.IsNullOrWhiteSpace(user.Job) || user.Job == "Sample")
        {
            user.Job = user.Email == DemoBeneficiary2Email ? "Teacher" : "Homemaker";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(user.MaritalStatus) || user.MaritalStatus == "Single")
        {
            user.MaritalStatus = "Married";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(user.EducationalStatus))
        {
            user.EducationalStatus = "Bachelor";
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(user.HealthStatus))
        {
            user.HealthStatus = "Good";
            changed = true;
        }

        if (user.DateOfBirth is null)
        {
            user.DateOfBirth = user.Email == DemoBeneficiary2Email
                ? new DateTime(1988, 3, 10)
                : new DateTime(1990, 8, 22);
            changed = true;
        }

        if (changed)
        {
            await userManager.UpdateAsync(user);
            logger.LogInformation("Enriched profile fields for demo user {Email}.", user.Email);
        }
    }

    private static async Task<Beneficiary> EnsureBeneficiaryAsync(
        AppDbContext context,
        User user,
        Guid areaId,
        string idPhotoUrl,
        bool isHeadOfHousehold,
        CancellationToken cancellationToken)
    {
        var existing = await context.Beneficiaries
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(b => b.UserId == user.Id && !b.IsDeleted, cancellationToken);

        if (existing is not null)
        {
            if (!existing.IsHeadOfHousehold && isHeadOfHousehold)
            {
                existing.IsHeadOfHousehold = true;
                await context.SaveChangesAsync(cancellationToken);
            }

            return existing;
        }

        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == user.Id, cancellationToken);

        if (address is null)
        {
            address = new Address
            {
                AreaId = areaId,
                UserId = user.Id,
                Street = $"Demo Street for {user.FirstName}",
                Details = "Seeded address for FamilyMembers testing",
                Latitude = 31.9000,
                Longitude = 35.2000
            };
            context.Addresses.Add(address);
            await context.SaveChangesAsync(cancellationToken);
        }

        var beneficiary = new Beneficiary
        {
            UserId = user.Id,
            AddressId = address.Id,
            IdPhotoUrl = idPhotoUrl,
            IsHeadOfHousehold = isHeadOfHousehold,
            VerificationStatus = VerificationStatus.Verified,
            VerifiedUntil = DateTime.UtcNow.AddMonths(6),
            IsDeleted = false
        };

        context.Beneficiaries.Add(beneficiary);
        await context.SaveChangesAsync(cancellationToken);
        return beneficiary;
>>>>>>> a211c529ed7431505e5bcb3244d053a964071773
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
