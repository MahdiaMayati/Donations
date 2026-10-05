using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemTypes.Commands.UpdateItemType;

public sealed class UpdateItemTypeCommandHandler
    : IRequestHandler<UpdateItemTypeCommand, ItemTypeResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateItemTypeCommandHandler> _logger;

    public UpdateItemTypeCommandHandler(
        IAppDbContext context,
        ILogger<UpdateItemTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemTypeResponse?> Handle(
        UpdateItemTypeCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.ItemTypes
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var category = await _context.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Item category not found.");
        }

        var name = request.Name.Trim();

        var duplicate = await _context.ItemTypes
            .AnyAsync(
                t => t.Id != request.Id
                     && t.CategoryId == request.CategoryId
                     && t.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (duplicate)
        {
            throw new ConflictException(
                "An item type with this name already exists in the selected category.");
        }

        entity.CategoryId = request.CategoryId;
        entity.Name = name;
        entity.OutfitUnits = request.OutfitUnits;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.Category = category;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemType updated with Id {ItemTypeId}", entity.Id);
        return ItemTypeMappings.ToResponse(entity);
    }
}
