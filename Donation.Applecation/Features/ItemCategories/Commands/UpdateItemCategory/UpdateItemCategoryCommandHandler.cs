using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemCategories.Commands.UpdateItemCategory;

public sealed class UpdateItemCategoryCommandHandler
    : IRequestHandler<UpdateItemCategoryCommand, ItemCategoryResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateItemCategoryCommandHandler> _logger;

    public UpdateItemCategoryCommandHandler(
        IAppDbContext context,
        ILogger<UpdateItemCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemCategoryResponse?> Handle(
        UpdateItemCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.ItemCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        var name = request.Name.Trim();

        var duplicate = await _context.ItemCategories
            .AnyAsync(
                c => c.Id != request.Id && c.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (duplicate)
        {
            throw new ConflictException("An item category with this name already exists.");
        }

        entity.Name = name;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemCategory updated with Id {ItemCategoryId}", entity.Id);
        return ItemCategoryMappings.ToResponse(entity);
    }
}
