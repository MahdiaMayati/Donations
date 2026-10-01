using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemTypes.Commands.RestoreItemType;

public sealed class RestoreItemTypeCommandHandler
    : IRequestHandler<RestoreItemTypeCommand, ItemTypeResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RestoreItemTypeCommandHandler> _logger;

    public RestoreItemTypeCommandHandler(
        IAppDbContext context,
        ILogger<RestoreItemTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemTypeResponse?> Handle(
        RestoreItemTypeCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.ItemTypes
            .IgnoreQueryFilters()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null || !entity.IsDeleted)
        {
            return null;
        }

        var categoryActive = await _context.ItemCategories
            .AnyAsync(c => c.Id == entity.CategoryId, cancellationToken);

        if (!categoryActive)
        {
            throw new BusinessRuleException(
                "Cannot restore this item type because its category is missing or soft-deleted. Restore the category first.");
        }

        var nameTaken = await _context.ItemTypes
            .AnyAsync(
                t => t.CategoryId == entity.CategoryId && t.Name.ToLower() == entity.Name.ToLower(),
                cancellationToken);

        if (nameTaken)
        {
            throw new ConflictException(
                "Cannot restore this item type because an active type with the same name already exists in the category.");
        }

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Reload category name if filter hid navigation
        if (entity.Category is null || entity.Category.IsDeleted)
        {
            entity.Category = await _context.ItemCategories
                .AsNoTracking()
                .FirstAsync(c => c.Id == entity.CategoryId, cancellationToken);
        }

        _logger.LogInformation("ItemType restored with Id {ItemTypeId}", entity.Id);
        return ItemTypeMappings.ToResponse(entity);
    }
}
