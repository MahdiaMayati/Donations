using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.City.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Cities.Queries.GetAllCities;

public sealed class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, PaginatedResult<CityResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllCitiesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<CityResponse>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Cities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(term) || c.Code.ToLower().Contains(term));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CityResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code
            })
            .ToPaginatedListAsync(request.Page, request.Limit, cancellationToken);
    }
}
