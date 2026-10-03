using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.Material.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.Materials.Queries.GetMaterialById;

public sealed class GetMaterialByIdQueryHandler : IRequestHandler<GetMaterialByIdQuery, MaterialResponse?>
{
    private readonly IAppDbContext _context;

    public GetMaterialByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MaterialResponse?> Handle(GetMaterialByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Materials
            .AsNoTracking()
            .Where(m => m.Id == request.Id)
            .Select(m => new MaterialResponse
            {
                Id = m.Id,
                Name = m.Name,
                IsDeleted = m.IsDeleted
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
