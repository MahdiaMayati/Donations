using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Items.Commands.DeleteItem;

public sealed class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(IAppDbContext context, ILogger<DeleteItemCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item is null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        item.IsDeleted = true;
        item.DeletedAt = now;
        item.UpdatedAt = now;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item soft-deleted with Id {ItemId}", request.Id);
        return true;
    }
}
