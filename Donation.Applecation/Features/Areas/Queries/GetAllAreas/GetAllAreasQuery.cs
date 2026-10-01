using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Area.Response;
using MediatR;

namespace Donation.Application.Features.Areas.Queries.GetAllAreas;

public sealed record GetAllAreasQuery(
    Guid? CityId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<AreaResponse>>;
