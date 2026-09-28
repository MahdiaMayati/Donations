using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.DeleteVolunteer;

public sealed record DeleteVolunteerCommand(int Id) : IRequest<bool>;
