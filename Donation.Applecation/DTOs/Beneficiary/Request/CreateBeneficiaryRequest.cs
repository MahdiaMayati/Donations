namespace Donation.Application.DTOs.Beneficiary.Request;

public sealed class CreateBeneficiaryRequest
{
    public Guid AddressId { get; set; }
    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }
}
