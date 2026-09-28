using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Beneficiary.Request;

public sealed class UpdateBeneficiaryRequest
{
    public int AddressId { get; set; }
    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public DateTime? VerifiedUntil { get; set; }
}
