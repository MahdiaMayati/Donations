using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Beneficiary.Response;

public sealed class BeneficiaryResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AddressId { get; set; }
    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime? VerifiedUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
