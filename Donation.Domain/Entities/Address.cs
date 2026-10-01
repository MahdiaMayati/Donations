namespace Donation.Domain.Entities;

public class Address
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AreaId { get; set; }
    public Guid UserId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Area Area { get; set; } = null!;
    public User User { get; set; } = null!;
}
