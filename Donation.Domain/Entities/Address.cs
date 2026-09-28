namespace Donation.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public int AreaId { get; set; }
    public Guid UserId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Area Area { get; set; } = null!;
    public User User { get; set; } = null!;
}
