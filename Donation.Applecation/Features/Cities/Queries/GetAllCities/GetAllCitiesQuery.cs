using Donation.Application.DTOs.City.Response;
using MediatR;

namespace Donation.Application.Features.Cities.Queries.GetAllCities;

public sealed record GetAllCitiesQuery : IRequest<IReadOnlyList<CityResponse>>;
