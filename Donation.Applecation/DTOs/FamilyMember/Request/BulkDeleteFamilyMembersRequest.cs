namespace Donation.Application.DTOs.FamilyMember.Request;

public sealed class BulkDeleteFamilyMembersRequest
{
    public List<Guid> Ids { get; set; } = new();
}
