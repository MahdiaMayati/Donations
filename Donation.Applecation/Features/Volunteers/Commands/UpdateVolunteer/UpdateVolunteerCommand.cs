using Donation.Application.DTOs.Volunteer.Request;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;

public sealed record UpdateVolunteerCommand(
    Guid Id,
    Guid? OrganizationId,
    string? Status,
    string? Days,
    int? HoursCount,
    string? Hobbies,
    string? Skills,
    VolunteerAddressRequest? Address) : IRequest<VolunteerResponse?>;
