using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class Volunteer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public VolunteerStatus Status { get; set; } = VolunteerStatus.Pending;

    public User User { get; set; } = null!;
}
