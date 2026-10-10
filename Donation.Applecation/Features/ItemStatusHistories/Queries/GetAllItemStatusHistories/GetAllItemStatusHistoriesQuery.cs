using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemStatusHistory.Response;
using MediatR;

namespace Donation.Application.Features.ItemStatusHistories.Queries.GetAllItemStatusHistories;

public sealed record GetAllItemStatusHistoriesQuery(
    Guid? ItemId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit) : IRequest<PaginatedResult<ItemStatusHistoryResponse>>;
