using Donation.Application.Abstractions.Persistence;
using Donation.Application.DTOs.StorageLocation.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Features.StorageLocations.Queries.GetStorageLocationById;

public sealed class GetStorageLocationByIdQueryHandler
    : IRequestHandler<GetStorageLocationByIdQuery, StorageLocationResponse?>
{
    private readonly IAppDbContext _context;

    public GetStorageLocationByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<StorageLocationResponse?> Handle(
        GetStorageLocationByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.StorageLocations
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => new StorageLocationResponse
            {
                Id = s.Id,
                WarehouseId = s.WarehouseId,
                Code = s.Code
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
