using MediatR;

namespace Donation.Application.Features.Cities.Commands.DeleteCity;

public sealed record DeleteCityCommand(Guid Id) : IRequest<bool>;
