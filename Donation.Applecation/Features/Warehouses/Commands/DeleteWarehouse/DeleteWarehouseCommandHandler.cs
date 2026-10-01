using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Warehouses.Commands.DeleteWarehouse;

public sealed class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteWarehouseCommandHandler> _logger;

    public DeleteWarehouseCommandHandler(
        IAppDbContext context,
        ILogger<DeleteWarehouseCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (warehouse is null)
        {
            return false;
        }

        var hasStorageLocations = await _context.StorageLocations
            .AnyAsync(s => s.WarehouseId == request.Id, cancellationToken);

        if (hasStorageLocations)
        {
            throw new ConflictException("Cannot delete this warehouse because it has related storage locations.");
        }

        warehouse.IsDeleted = true;
        warehouse.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Warehouse soft-deleted with Id {WarehouseId}", request.Id);

        return true;
    }
}
