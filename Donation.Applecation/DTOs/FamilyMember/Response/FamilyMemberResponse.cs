using Donation.Domain.Enums;

namespace Donation.Application.DTOs.FamilyMember.Response;

public sealed class FamilyMemberResponse
{
    public Guid Id { get; set; }
    public Guid BeneficiaryId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Gender Gender { get; set; }
    public ClothingSize ClothingSize { get; set; }
    public string ShoeSize { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}
