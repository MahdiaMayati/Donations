namespace Donation.Application.DTOs.ItemType.Request;

public sealed class UpdateItemTypeRequest
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal OutfitUnits { get; set; }
}
