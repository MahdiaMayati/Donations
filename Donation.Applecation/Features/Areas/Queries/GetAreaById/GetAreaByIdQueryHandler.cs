using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Area.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Areas.Queries.GetAreaById;

public sealed class GetAreaByIdQueryHandler : IRequestHandler<GetAreaByIdQuery, AreaResponse?>
{
    private readonly IAppDbContext _context;

    public GetAreaByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<AreaResponse?> Handle(GetAreaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Areas
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .Select(a => new AreaResponse
            {
                Id = a.Id,
                CityId = a.CityId,
                Name = a.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
