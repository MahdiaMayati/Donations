namespace Donation.Application.DTOs.ItemColor.Response;

public sealed class ItemColorResponse
{
    public Guid ItemId { get; set; }
    public Guid ColorId { get; set; }
    public string? ColorName { get; set; }
    public string? ColorCode { get; set; }
}
