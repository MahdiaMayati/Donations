using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Beneficiary.Response;

/// <summary>
/// Uniform beneficiary profile payload for list, get-by-id, create, and update responses.
/// </summary>
public sealed class BeneficiaryResponse
{
    // --- System-managed ---
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public VerificationStatus VerificationStatus { get; set; }

    /// <summary>Derived from <see cref="VerificationStatus"/>; not stored separately.</summary>
    public bool IsVerified { get; set; }

    public DateTime? VerifiedUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // --- User / register profile ---
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    public string PreferredContactMethod { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EducationalStatus { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;
    public Guid? OrganizationId { get; set; }

    // --- Location (city name only — no city id/object) ---
    public Guid AddressId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressDetails { get; set; } = string.Empty;

    // --- Beneficiary-specific input fields ---
    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }
}
