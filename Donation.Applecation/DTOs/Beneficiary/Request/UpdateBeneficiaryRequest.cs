using Donation.Domain.Enums;

namespace Donation.Application.DTOs.Beneficiary.Request;

/// <summary>
/// Client-updatable beneficiary fields. UserId / CreatedAt / IsDeleted are system-owned.
/// <see cref="VerificationStatus"/> and <see cref="VerifiedUntil"/> are applied only for Admin callers.
/// </summary>
public sealed class UpdateBeneficiaryRequest
{
    public Guid AddressId { get; set; }
    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }

    /// <summary>Admin only — ignored for non-admin callers.</summary>
    public VerificationStatus? VerificationStatus { get; set; }

    /// <summary>Admin only — ignored for non-admin callers.</summary>
    public DateTime? VerifiedUntil { get; set; }
}
