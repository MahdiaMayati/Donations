namespace Donation.Domain.Entities;

public class Material
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
