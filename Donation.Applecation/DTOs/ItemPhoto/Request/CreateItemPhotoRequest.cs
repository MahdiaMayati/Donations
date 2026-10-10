namespace Donation.Application.DTOs.ItemPhoto.Request;

public sealed class CreateItemPhotoRequest
{
    public Guid ItemId { get; set; }
    public string Url { get; set; } = string.Empty;
}
