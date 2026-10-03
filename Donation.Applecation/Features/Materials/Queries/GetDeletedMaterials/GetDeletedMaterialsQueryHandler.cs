using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Material.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Materials.Queries.GetDeletedMaterials;

public sealed class GetDeletedMaterialsQueryHandler
    : IRequestHandler<GetDeletedMaterialsQuery, PaginatedResult<MaterialResponse>>
{
    private readonly IAppDbContext _context;

    public GetDeletedMaterialsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<MaterialResponse>> Handle(
        GetDeletedMaterialsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Materials
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(m => m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(m => m.Name)
            .Select(m => new MaterialResponse
            {
                Id = m.Id,
                Name = m.Name,
                IsDeleted = m.IsDeleted
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
