namespace Donation.Application.DTOs.ItemColor.Request;

public sealed class CreateItemColorRequest
{
    public Guid ItemId { get; set; }
    public Guid ColorId { get; set; }
}
