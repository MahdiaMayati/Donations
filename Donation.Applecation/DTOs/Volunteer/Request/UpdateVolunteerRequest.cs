namespace Donation.Application.DTOs.Volunteer.Request;

public sealed class UpdateVolunteerRequest
{
    public Guid? OrganizationId { get; set; }
    public string? Status { get; set; }
    public string? Days { get; set; }
    public int? HoursCount { get; set; }
    public string? Hobbies { get; set; }
    public string? Skills { get; set; }
    public string? Experiences { get; set; }
    public int? NeglectedTasksCount { get; set; }
    public VolunteerAddressRequest? Address { get; set; }
}
