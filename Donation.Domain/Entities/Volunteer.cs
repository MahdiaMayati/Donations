namespace Donation.Domain.Entities;

public class Volunteer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public string Status { get; set; } = "Active";
    public string Days { get; set; } = string.Empty;
    public int HoursCount { get; set; }
    public string? Hobbies { get; set; }
    public string? Skills { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
