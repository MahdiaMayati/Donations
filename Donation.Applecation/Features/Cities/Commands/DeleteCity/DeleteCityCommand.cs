using MediatR;

namespace Donation.Application.Features.Cities.Commands.DeleteCity;

public sealed record DeleteCityCommand(int Id) : IRequest<bool>;
