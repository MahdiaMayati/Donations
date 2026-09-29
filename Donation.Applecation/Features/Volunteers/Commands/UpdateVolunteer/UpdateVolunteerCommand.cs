using Donation.Application.DTOs.Volunteer.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;

public sealed record UpdateVolunteerCommand(int Id, VolunteerStatus Status) : IRequest<VolunteerResponse?>;
