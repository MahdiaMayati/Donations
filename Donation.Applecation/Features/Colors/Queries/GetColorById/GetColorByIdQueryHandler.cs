using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Color.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Colors.Queries.GetColorById;

public sealed class GetColorByIdQueryHandler : IRequestHandler<GetColorByIdQuery, ColorResponse?>
{
    private readonly IAppDbContext _context;

    public GetColorByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ColorResponse?> Handle(GetColorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Colors
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new ColorResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                IsDeleted = c.IsDeleted
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
