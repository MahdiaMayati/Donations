using MediatR;

namespace Donation.Application.Features.Volunteers.Commands.DeleteVolunteer;

public sealed record DeleteVolunteerCommand(Guid Id) : IRequest<bool>;
