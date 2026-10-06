namespace Donation.Application.DTOs.ItemPhoto.Response;

public sealed class ItemPhotoResponse
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
