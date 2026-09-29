using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class Volunteer
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public VolunteerStatus Status { get; set; } = VolunteerStatus.Pending;

    public User User { get; set; } = null!;
}
