namespace Donation.Application.DTOs.Volunteer.Request;

public sealed class CreateVolunteerRequest
{
    public Guid OrganizationId { get; set; }
    public string? Status { get; set; }
    public string Days { get; set; } = string.Empty;
    public int HoursCount { get; set; }
    public string? Hobbies { get; set; }
    public string? Skills { get; set; }
    public VolunteerAddressRequest Address { get; set; } = null!;
}
