using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Area.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Areas.Queries.GetAllAreas;

public sealed class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, IReadOnlyList<AreaResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllAreasQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AreaResponse>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Areas.AsNoTracking();

        if (request.CityId.HasValue)
        {
            query = query.Where(a => a.CityId == request.CityId.Value);
        }

        return await query
            .OrderBy(a => a.Name)
            .Select(a => new AreaResponse
            {
                Id = a.Id,
                CityId = a.CityId,
                Name = a.Name
            })
            .ToListAsync(cancellationToken);
    }
}
