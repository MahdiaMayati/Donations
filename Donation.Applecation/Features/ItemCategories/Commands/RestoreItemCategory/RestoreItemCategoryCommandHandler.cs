using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemCategories.Commands.RestoreItemCategory;

public sealed class RestoreItemCategoryCommandHandler
    : IRequestHandler<RestoreItemCategoryCommand, ItemCategoryResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<RestoreItemCategoryCommandHandler> _logger;

    public RestoreItemCategoryCommandHandler(
        IAppDbContext context,
        ILogger<RestoreItemCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemCategoryResponse?> Handle(
        RestoreItemCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.ItemCategories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity is null || !entity.IsDeleted)
        {
            return null;
        }

        var nameTaken = await _context.ItemCategories
            .AnyAsync(c => c.Name.ToLower() == entity.Name.ToLower(), cancellationToken);

        if (nameTaken)
        {
            throw new ConflictException(
                "Cannot restore this item category because an active category with the same name already exists.");
        }

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemCategory restored with Id {ItemCategoryId}", entity.Id);
        return ItemCategoryMappings.ToResponse(entity);
    }
}
