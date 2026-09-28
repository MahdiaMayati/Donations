using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Volunteer.Response;

public sealed class VolunteerResponse
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public VolunteerStatus Status { get; set; }
}
