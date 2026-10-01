using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemTypes.Queries.GetDeletedItemTypes;

public sealed class GetDeletedItemTypesQueryHandler
    : IRequestHandler<GetDeletedItemTypesQuery, PaginatedResult<ItemTypeResponse>>
{
    private readonly IAppDbContext _context;

    public GetDeletedItemTypesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemTypeResponse>> Handle(
        GetDeletedItemTypesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ItemTypes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.IsDeleted);

        if (request.CategoryId is Guid categoryId)
        {
            query = query.Where(t => t.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(t => t.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(t => t.Name)
            .Select(t => new ItemTypeResponse
            {
                Id = t.Id,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
                Name = t.Name,
                OutfitUnits = t.OutfitUnits,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
