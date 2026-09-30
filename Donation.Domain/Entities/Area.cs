namespace Donation.Domain.Entities;

public class Area
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CityId { get; set; }
    public string Name { get; set; } = string.Empty;

    public City City { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
