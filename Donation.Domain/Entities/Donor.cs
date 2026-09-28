namespace Donation.Domain.Entities;

public class Donor
{
    public int Id { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
