using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Volunteer.Request;

public sealed class UpdateVolunteerRequest
{
    public VolunteerStatus Status { get; set; }
}
