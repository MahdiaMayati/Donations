using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.City.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Cities.Queries.GetAllCities;

public sealed class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, IReadOnlyList<CityResponse>>
{
    private readonly IAppDbContext _context;

    public GetAllCitiesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CityResponse>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Cities
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CityResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code
            })
            .ToListAsync(cancellationToken);
    }
}
