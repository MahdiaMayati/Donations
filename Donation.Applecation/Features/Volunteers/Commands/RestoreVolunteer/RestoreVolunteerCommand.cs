using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.RestoreVolunteer;

public sealed record RestoreVolunteerCommand(Guid Id) : IRequest<VolunteerResponse?>;
