using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class Volunteer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public VolunteerStatus Status { get; set; } = VolunteerStatus.Pending;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
}
