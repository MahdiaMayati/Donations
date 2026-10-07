namespace Donation.Domain.Entities;

public class Color
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public ICollection<ItemColor> ItemColors { get; set; } = new List<ItemColor>();
}
