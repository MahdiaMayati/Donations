namespace Donation.Application.DTOs.Auth.Request;

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }

    public DateTime? DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female. Required on registration.</summary>
    public bool? Gender { get; set; }

    /// <summary>Suggested values: WhatsApp, Call, SMS.</summary>
    public string PreferredContactMethod { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EducationalStatus { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;
}
