using Donation.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemTypes.Commands.DeleteItemType;

public sealed class DeleteItemTypeCommandHandler : IRequestHandler<DeleteItemTypeCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteItemTypeCommandHandler> _logger;

    public DeleteItemTypeCommandHandler(
        IAppDbContext context,
        ILogger<DeleteItemTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ItemTypes
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemType soft-deleted with Id {ItemTypeId}", request.Id);
        return true;
    }
}
