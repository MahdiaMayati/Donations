using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemType.Response;
using MediatR;

namespace Donation.Application.Features.ItemTypes.Queries.GetDeletedItemTypes;

public sealed record GetDeletedItemTypesQuery(
    Guid? CategoryId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<ItemTypeResponse>>;
