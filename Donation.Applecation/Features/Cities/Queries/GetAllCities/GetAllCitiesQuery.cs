using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.City.Response;
using MediatR;

namespace Donation.Application.Features.Cities.Queries.GetAllCities;

public sealed record GetAllCitiesQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<CityResponse>>;
