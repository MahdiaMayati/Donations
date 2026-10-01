using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.StorageLocation.Response;
using Donation.Application.Features.StorageLocations.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.StorageLocations.Commands.UpdateStorageLocation;

public sealed class UpdateStorageLocationCommandHandler
    : IRequestHandler<UpdateStorageLocationCommand, StorageLocationResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateStorageLocationCommandHandler> _logger;

    public UpdateStorageLocationCommandHandler(
        IAppDbContext context,
        ILogger<UpdateStorageLocationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StorageLocationResponse?> Handle(
        UpdateStorageLocationCommand request,
        CancellationToken cancellationToken)
    {
        var location = await _context.StorageLocations
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (location is null)
        {
            return null;
        }

        var warehouseExists = await _context.Warehouses
            .AnyAsync(w => w.Id == request.WarehouseId && !w.IsDeleted, cancellationToken);

        if (!warehouseExists)
        {
            throw new NotFoundException("Warehouse not found.");
        }

        var code = request.Code.Trim();

        var codeExists = await _context.StorageLocations
            .AnyAsync(
                s => s.Id != request.Id
                     && s.WarehouseId == request.WarehouseId
                     && s.Code.ToLower() == code.ToLower(),
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException("A storage location with this code already exists for the warehouse.");
        }

        location.WarehouseId = request.WarehouseId;
        location.Code = code;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("StorageLocation updated with Id {StorageLocationId}", location.Id);

        return StorageLocationMapper.Map(location);
    }
}
