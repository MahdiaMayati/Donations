using Donation.Domain.Enums;

namespace Donation.Application.DTOs.DonationRequest.Request;

public sealed class DonationRequestItemRequest
{
    /// <summary>Null/empty on create; required for updating an existing nested item.</summary>
    public Guid? Id { get; set; }
    public Guid ItemTypeId { get; set; }
    public Guid? MaterialId { get; set; }
    public string? Barcode { get; set; }
    public TargetGender TargetGender { get; set; }
    public AgeGroup AgeGroup { get; set; }
    public string Size { get; set; } = string.Empty;
    public Season Season { get; set; }
    public ItemCondition Condition { get; set; }
    public List<Guid> ColorIds { get; set; } = new();
    public List<string> PhotoUrls { get; set; } = new();
}
