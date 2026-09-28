using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.CreateVolunteer;

public sealed record CreateVolunteerCommand() : IRequest<VolunteerResponse>;
