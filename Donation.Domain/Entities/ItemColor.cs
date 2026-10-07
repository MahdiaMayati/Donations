namespace Donation.Domain.Entities;

public class ItemColor
{
    public Guid ItemId { get; set; }
    public Guid ColorId { get; set; }

    public Item Item { get; set; } = null!;
    public Color Color { get; set; } = null!;
}
