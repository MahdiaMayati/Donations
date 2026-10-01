namespace Donation.Application.DTOs.Beneficiary.Request;

/// <summary>
/// Combined beneficiary registration payload: user profile + city/area/address.
/// UserId is always taken from the authenticated caller — never from the client.
/// </summary>
public sealed class CreateBeneficiaryRequest
{
    public CreateBeneficiaryUserRequest User { get; set; } = new();
    public CreateBeneficiaryCityRequest City { get; set; } = new();
    public CreateBeneficiaryAreaRequest Area { get; set; } = new();
    public CreateBeneficiaryAddressRequest Address { get; set; } = new();

    public string IdPhotoUrl { get; set; } = string.Empty;
    public bool IsHeadOfHousehold { get; set; }
}

/// <summary>
/// Profile fields applied to the authenticated user during beneficiary creation.
/// Email/password are not accepted here — account creation remains on /api/Auth/register.
/// </summary>
public sealed class CreateBeneficiaryUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }

    /// <summary>true = Male, false = Female.</summary>
    public bool Gender { get; set; }

    /// <summary>Suggested values: WhatsApp, Call, SMS.</summary>
    public string PreferredContactMethod { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string EducationalStatus { get; set; } = string.Empty;
    public string Job { get; set; } = string.Empty;
    public string HealthStatus { get; set; } = string.Empty;
}

/// <summary>
/// Links an existing city by id only (city is never created via beneficiary registration).
/// </summary>
public sealed class CreateBeneficiaryCityRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Area name used to find or create an area under the selected city.
/// </summary>
public sealed class CreateBeneficiaryAreaRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Address fields used to find or create an address under the resolved area.
/// </summary>
public sealed class CreateBeneficiaryAddressRequest
{
    public string Street { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
