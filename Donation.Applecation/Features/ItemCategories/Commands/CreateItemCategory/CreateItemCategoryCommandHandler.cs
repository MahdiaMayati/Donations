using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.ItemCategory.Response;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.ItemCategories.Commands.CreateItemCategory;

public sealed class CreateItemCategoryCommandHandler
    : IRequestHandler<CreateItemCategoryCommand, ItemCategoryResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateItemCategoryCommandHandler> _logger;

    public CreateItemCategoryCommandHandler(
        IAppDbContext context,
        ILogger<CreateItemCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ItemCategoryResponse> Handle(
        CreateItemCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var exists = await _context.ItemCategories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);

        if (exists)
        {
            throw new ConflictException("An item category with this name already exists.");
        }

        var entity = new ItemCategory
        {
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        _context.ItemCategories.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ItemCategory created with Id {ItemCategoryId}", entity.Id);
        return ItemCategoryMappings.ToResponse(entity);
    }
}
