using Donation.Application.DTOs.Volunteer.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.Volunteers;

internal static class VolunteerMapping
{
    public static readonly string[] AllowedStatuses =
    [
        "Active",
        "Pending",
        "Inactive",
        "Suspended"
    ];

    public static VolunteerResponse ToResponse(
        Volunteer volunteer,
        User user,
        Organization? organization,
        Address? address) => new()
    {
        Id = volunteer.Id,
        UserId = volunteer.UserId,
        OrganizationId = volunteer.OrganizationId,
        OrganizationName = organization?.Name ?? string.Empty,
        Status = volunteer.Status ?? string.Empty,
        Days = volunteer.Days ?? string.Empty,
        HoursCount = volunteer.HoursCount,
        Hobbies = volunteer.Hobbies,
        Skills = volunteer.Skills,
        Experiences = volunteer.Experiences,
        NeglectedTasksCount = volunteer.NeglectedTasksCount,
        FullName = $"{user.FirstName} {user.LastName}".Trim(),
        Email = user.Email ?? string.Empty,
        PhoneNumber = user.PhoneNumber,
        PreferredContactMethod = user.PreferredContactMethod,
        Address = address is null ? null : ToAddressResponse(address)
    };

    public static VolunteerAddressResponse ToAddressResponse(Address address) => new()
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

    public static string NormalizeStatus(string? status)
        => string.IsNullOrWhiteSpace(status) ? "Active" : status.Trim();
}
