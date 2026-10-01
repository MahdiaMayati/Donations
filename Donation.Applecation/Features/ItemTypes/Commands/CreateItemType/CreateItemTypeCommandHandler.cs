using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemType.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemTypes.Commands.CreateItemType;

public sealed class CreateItemTypeCommandHandler
    : IRequestHandler<CreateItemTypeCommand, ItemTypeResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateItemTypeCommandHandler> _logger;

    public CreateItemTypeCommandHandler(
        IAppDbContext context,
        ILogger<CreateItemTypeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemTypeResponse> Handle(
        CreateItemTypeCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException("Item category not found.");
        }

        var name = request.Name.Trim();

        var exists = await _context.ItemTypes
            .AnyAsync(
                t => t.CategoryId == request.CategoryId && t.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (exists)
        {
            throw new ConflictException(
                "An item type with this name already exists in the selected category.");
        }

        var entity = new ItemType
        {
            CategoryId = request.CategoryId,
            Name = name,
            OutfitUnits = request.OutfitUnits,
            CreatedAt = DateTime.UtcNow,
            Category = category
        };

        _context.ItemTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemType created with Id {ItemTypeId}", entity.Id);
        return ItemTypeMappings.ToResponse(entity);
    }
}
