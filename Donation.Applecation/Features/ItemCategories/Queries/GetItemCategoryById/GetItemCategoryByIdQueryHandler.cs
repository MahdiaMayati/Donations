using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemCategories.Queries.GetItemCategoryById;

public sealed class GetItemCategoryByIdQueryHandler
    : IRequestHandler<GetItemCategoryByIdQuery, ItemCategoryResponse?>
{
    private readonly IAppDbContext _context;

    public GetItemCategoryByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ItemCategoryResponse?> Handle(
        GetItemCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.ItemCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        return entity is null ? null : ItemCategoryMappings.ToResponse(entity);
    }
}
