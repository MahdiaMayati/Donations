using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Area.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Areas.Queries.GetAllAreas;

public sealed class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, PaginatedResult<AreaResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllAreasQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<AreaResponse>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Areas.AsNoTracking();

        if (request.CityId.HasValue)
        {
            query = query.Where(a => a.CityId == request.CityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(term));
        }

        return await query
            .OrderBy(a => a.Name)
            .Select(a => new AreaResponse
            {
                Id = a.Id,
                CityId = a.CityId,
                Name = a.Name
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
