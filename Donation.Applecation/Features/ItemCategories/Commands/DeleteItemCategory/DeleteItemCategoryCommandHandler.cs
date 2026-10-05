using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemCategories.Commands.DeleteItemCategory;

public sealed class DeleteItemCategoryCommandHandler : IRequestHandler<DeleteItemCategoryCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteItemCategoryCommandHandler> _logger;

    public DeleteItemCategoryCommandHandler(
        IAppDbContext context,
        ILogger<DeleteItemCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteItemCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        var hasActiveTypes = await _context.ItemTypes
            .AnyAsync(t => t.CategoryId == request.Id, cancellationToken);

        if (hasActiveTypes)
        {
            throw new BusinessRuleException(
                "Cannot delete this item category because it still has active item types.");
        }

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemCategory soft-deleted with Id {ItemCategoryId}", request.Id);
        return true;
    }
}
