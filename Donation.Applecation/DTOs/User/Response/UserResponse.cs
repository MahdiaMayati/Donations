namespace Donation.Application.DTOs.User.Response;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? OrganizationId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    public string PreferredContactMethod { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EducationalStatus { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;
}
