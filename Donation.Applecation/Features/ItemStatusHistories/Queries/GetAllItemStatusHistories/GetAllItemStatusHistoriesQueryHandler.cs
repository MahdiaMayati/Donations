using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemStatusHistory.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemStatusHistories.Queries.GetAllItemStatusHistories;

public sealed class GetAllItemStatusHistoriesQueryHandler
    : IRequestHandler<GetAllItemStatusHistoriesQuery, PaginatedResult<ItemStatusHistoryResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllItemStatusHistoriesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemStatusHistoryResponse>> Handle(
        GetAllItemStatusHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ItemStatusHistories.AsNoTracking();

        if (request.ItemId is Guid itemId && itemId != Guid.Empty)
        {
            query = query.Where(h => h.ItemId == itemId);
        }

        return await query
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new ItemStatusHistoryResponse
            {
                Id = h.Id,
                ItemId = h.ItemId,
                OldStatus = h.OldStatus.HasValue ? h.OldStatus.Value.ToString() : null,
                NewStatus = h.NewStatus.ToString(),
                ChangedAt = h.ChangedAt,
                ChangedByUserId = h.ChangedByUserId
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
