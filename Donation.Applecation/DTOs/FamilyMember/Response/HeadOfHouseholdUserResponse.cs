namespace Donation.Application.DTOs.FamilyMember.Response;

/// <summary>
/// Full User entity fields for Head of Household (excludes secrets like PasswordHash/SecurityStamp).
/// </summary>
public sealed class HeadOfHouseholdUserResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? OrganizationId { get; set; }
    public DateTime? DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female (User.Gender bool).</summary>
    public bool Gender { get; set; }

    public string PreferredContactMethod { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EducationalStatus { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;
}
