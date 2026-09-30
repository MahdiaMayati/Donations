using Donation.Application.DTOs.City.Response;
using MediatR;

namespace Donation.Application.Features.Cities.Commands.UpdateCity;

public sealed record UpdateCityCommand(Guid Id, string Name, string Code) : IRequest<CityResponse?>;
