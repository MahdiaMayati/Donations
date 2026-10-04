using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Queries.GetAllColors;

public sealed record GetAllColorsQuery(
    IReadOnlyList<Guid>? Ids = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<ColorResponse>>;
