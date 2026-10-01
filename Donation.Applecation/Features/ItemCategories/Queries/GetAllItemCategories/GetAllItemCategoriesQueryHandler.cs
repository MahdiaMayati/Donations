using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemCategory.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.ItemCategories.Queries.GetAllItemCategories;

public sealed class GetAllItemCategoriesQueryHandler
    : IRequestHandler<GetAllItemCategoriesQuery, PaginatedResult<ItemCategoryResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllItemCategoriesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<ItemCategoryResponse>> Handle(
        GetAllItemCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.ItemCategories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ItemCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
