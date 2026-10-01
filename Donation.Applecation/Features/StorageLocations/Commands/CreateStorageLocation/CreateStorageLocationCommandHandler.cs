using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.StorageLocation.Response;
using Donation.Application.Features.StorageLocations.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.StorageLocations.Commands.CreateStorageLocation;

public sealed class CreateStorageLocationCommandHandler
    : IRequestHandler<CreateStorageLocationCommand, StorageLocationResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateStorageLocationCommandHandler> _logger;

    public CreateStorageLocationCommandHandler(
        IAppDbContext context,
        ILogger<CreateStorageLocationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StorageLocationResponse> Handle(
        CreateStorageLocationCommand request,
        CancellationToken cancellationToken)
    {
        var warehouseExists = await _context.Warehouses
            .AnyAsync(w => w.Id == request.WarehouseId && !w.IsDeleted, cancellationToken);

        if (!warehouseExists)
        {
            throw new NotFoundException("Warehouse not found.");
        }

        var code = request.Code.Trim();

        var codeExists = await _context.StorageLocations
            .AnyAsync(
                s => s.WarehouseId == request.WarehouseId
                     && s.Code.ToLower() == code.ToLower(),
                cancellationToken);

        if (codeExists)
        {
            throw new ConflictException("A storage location with this code already exists for the warehouse.");
        }

        var location = new StorageLocation
        {
            WarehouseId = request.WarehouseId,
            Code = code
        };

        _context.StorageLocations.Add(location);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("StorageLocation created with Id {StorageLocationId}", location.Id);

        return StorageLocationMapper.Map(location);
    }
}
