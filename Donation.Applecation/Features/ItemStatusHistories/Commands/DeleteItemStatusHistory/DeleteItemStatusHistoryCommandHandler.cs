using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemStatusHistories.Commands.DeleteItemStatusHistory;

public sealed class DeleteItemStatusHistoryCommandHandler
    : IRequestHandler<DeleteItemStatusHistoryCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteItemStatusHistoryCommandHandler> _logger;

    public DeleteItemStatusHistoryCommandHandler(
        IAppDbContext context,
        ILogger<DeleteItemStatusHistoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(
        DeleteItemStatusHistoryCommand request,
        CancellationToken cancellationToken)
    {
        var history = await _context.ItemStatusHistories
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (history is null)
        {
            return false;
        }

        _context.ItemStatusHistories.Remove(history);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemStatusHistory hard-deleted with Id {HistoryId}", request.Id);
        return true;
    }
}
