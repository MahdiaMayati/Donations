using Donation.Domain.Enums;

namespace Donation.Application.DTOs.FamilyMember.Request;

public sealed class CreateFamilyMemberRequest
{
    public int BeneficiaryId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public ClothingSize ClothingSize { get; set; }
    public string ShoeSize { get; set; } = string.Empty;
}
