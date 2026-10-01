namespace Donation.Domain.Entities;

public class FamilyMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BeneficiaryId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    public string ClothingSize { get; set; } = string.Empty;

    public string ShoeSize { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public Beneficiary Beneficiary { get; set; } = null!;
}
