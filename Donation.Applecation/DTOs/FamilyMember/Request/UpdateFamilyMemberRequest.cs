namespace Donation.Application.DTOs.FamilyMember.Request;

public sealed class UpdateFamilyMemberRequest
{
    /// <summary>Required for bulk update; ignored for single PUT when id is in the route.</summary>
    public Guid? Id { get; set; }

    /// <summary>User Id of the Head of Household (Beneficiary.UserId).</summary>
    public Guid HeadOfHouseholdId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    public string ClothingSize { get; set; } = string.Empty;

    public string ShoeSize { get; set; } = string.Empty;
}
