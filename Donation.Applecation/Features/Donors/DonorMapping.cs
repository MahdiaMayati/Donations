using Donation.Application.DTOs.Donor.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.Donors;

internal static class DonorMapping
{
    public static DonorResponse ToResponse(Donor donor, User user, Address? address) => new()
    {
        Id = donor.Id,
        UserId = donor.UserId,
        FullName = $"{user.FirstName} {user.LastName}".Trim(),
        Email = user.Email ?? string.Empty,
        PhoneNumber = user.PhoneNumber,
        PreferredContactMethod = user.PreferredContactMethod,
        Address = address is null ? null : ToAddressResponse(address)
    };

    public static DonorAddressResponse ToAddressResponse(Address address) => new()
    {
        Id = address.Id,
        AreaId = address.AreaId,
        AreaName = address.Area?.Name ?? string.Empty,
        CityName = address.Area?.City?.Name ?? string.Empty,
        Street = address.Street,
        Details = address.Details,
        Latitude = address.Latitude,
        Longitude = address.Longitude
    };

    public static (string FirstName, string LastName) SplitFullName(string fullName)
    {
        var trimmed = fullName.Trim();
        var parts = trimmed.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return (parts[0], parts[0]);
        }

        return (parts[0], parts[1]);
    }
}
