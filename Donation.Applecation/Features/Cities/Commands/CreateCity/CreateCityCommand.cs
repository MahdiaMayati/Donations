using Donation.Application.DTOs.City.Response;
using MediatR;

namespace Donation.Application.Features.Cities.Commands.CreateCity;

public sealed record CreateCityCommand(string Name, string Code) : IRequest<CityResponse>;
