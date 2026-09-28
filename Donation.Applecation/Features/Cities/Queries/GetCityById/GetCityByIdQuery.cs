using Donation.Application.DTOs.City.Response;
using MediatR;

namespace Donation.Application.Features.Cities.Queries.GetCityById;

public sealed record GetCityByIdQuery(int Id) : IRequest<CityResponse?>;
