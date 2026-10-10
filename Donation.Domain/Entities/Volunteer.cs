namespace Donation.Domain.Entities;

public class Volunteer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
<<<<<<< HEAD
    public Guid OrganizationId { get; set; }
    public string Status { get; set; } = "Active";
    public string Days { get; set; } = string.Empty;
    public int HoursCount { get; set; }
    public string? Hobbies { get; set; }
    public string? Skills { get; set; }
    public string? Experiences { get; set; }
    public int NeglectedTasksCount { get; set; }
=======
    public VolunteerStatus Status { get; set; } = VolunteerStatus.Pending;
>>>>>>> a211c529ed7431505e5bcb3244d053a964071773
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
