using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.ItemStatusHistory.Response;
using Donation.Application.Features.ItemStatusHistories.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemStatusHistories.Queries.GetItemStatusHistoryById;

public sealed class GetItemStatusHistoryByIdQueryHandler
    : IRequestHandler<GetItemStatusHistoryByIdQuery, ItemStatusHistoryResponse?>
{
    private readonly IAppDbContext _context;

    public GetItemStatusHistoryByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ItemStatusHistoryResponse?> Handle(
        GetItemStatusHistoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var history = await _context.ItemStatusHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        return history is null ? null : ItemStatusHistoryMapper.Map(history);
    }
}
