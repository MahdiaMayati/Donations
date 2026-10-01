namespace Donation.Application.DTOs.FamilyMember.Response;

public sealed class FamilyMemberResponse
{
    public Guid Id { get; set; }

    /// <summary>Beneficiary PK that owns this family member.</summary>
    public Guid BeneficiaryId { get; set; }

    /// <summary>Head of Household User Id (Beneficiary.UserId).</summary>
    public Guid HeadOfHouseholdId { get; set; }

    /// <summary>Same as HeadOfHouseholdId — User Id of the beneficiary / HoH.</summary>
    public Guid UserId { get; set; }

    public Guid AddressId { get; set; }

    /// <summary>Beneficiary IdPhotoUrl.</summary>
    public string ImageUrl { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    /// <summary>Maps from FamilyMember.BirthDate.</summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>Alias kept for older clients / projections.</summary>
    public DateTime BirthDate { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    public string ClothingSize { get; set; } = string.Empty;

    public string ShoeSize { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public HeadOfHouseholdUserResponse? HeadOfHousehold { get; set; }
}
