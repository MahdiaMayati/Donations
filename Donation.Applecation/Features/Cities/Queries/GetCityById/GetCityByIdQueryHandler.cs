using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.City.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Cities.Queries.GetCityById;

public sealed class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, CityResponse?>
{
    private readonly IAppDbContext _context;

    public GetCityByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CityResponse?> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Cities
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CityResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
