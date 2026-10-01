using Donation.Domain.Enums;

namespace Donation.Domain.Entities;

public class Beneficiary
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid AddressId { get; set; }
    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public DateTime? VerifiedUntil { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public User User { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
}
