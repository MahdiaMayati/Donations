namespace Donation.Application.DTOs.Volunteer.Response;

public sealed class VolunteerResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Days { get; set; } = string.Empty;
    public int HoursCount { get; set; }
    public string? Hobbies { get; set; }
    public string? Skills { get; set; }
    public string? Experiences { get; set; }
    public int NeglectedTasksCount { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PreferredContactMethod { get; set; } = string.Empty;
    public VolunteerAddressResponse? Address { get; set; }
}
