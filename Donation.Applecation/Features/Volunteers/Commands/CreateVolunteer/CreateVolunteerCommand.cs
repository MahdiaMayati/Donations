using Donation.Application.DTOs.Volunteer.Request;
using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.CreateVolunteer;

public sealed record CreateVolunteerCommand(
    Guid OrganizationId,
    string? Status,
    string Days,
    int HoursCount,
    string? Hobbies,
    string? Skills,
    VolunteerAddressRequest Address) : IRequest<VolunteerResponse>;
