using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Material.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Materials.Queries.GetAllMaterials;

public sealed class GetAllMaterialsQueryHandler
    : IRequestHandler<GetAllMaterialsQuery, PaginatedResult<MaterialResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllMaterialsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<MaterialResponse>> Handle(
        GetAllMaterialsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Materials.AsNoTracking();

        if (request.Ids is { Count: > 0 })
        {
            var ids = request.Ids.Where(id => id != Guid.Empty).Distinct().ToHashSet();
            query = query.Where(m => ids.Contains(m.Id));
        }

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
