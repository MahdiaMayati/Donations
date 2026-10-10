using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemType.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemTypes.Queries.GetAllItemTypes;

public sealed class GetAllItemTypesQueryHandler
    : IRequestHandler<GetAllItemTypesQuery, PaginatedResult<ItemTypeResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllItemTypesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemTypeResponse>> Handle(
        GetAllItemTypesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ItemTypes
            .AsNoTracking()
            .Include(t => t.Category)
            .AsQueryable();

        if (request.CategoryId is Guid categoryId)
        {
            query = query.Where(t => t.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(term) ||
                t.Category.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(t => t.Category.Name)
            .ThenBy(t => t.Name)
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
