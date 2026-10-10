using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Warehouse.Response;
using Donation.Application.Features.Warehouses.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Warehouses.Commands.UpdateWarehouse;

public sealed class UpdateWarehouseCommandHandler
    : IRequestHandler<UpdateWarehouseCommand, WarehouseResponse?>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateWarehouseCommandHandler> _logger;

    public UpdateWarehouseCommandHandler(
        IAppDbContext context,
        ILogger<UpdateWarehouseCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WarehouseResponse?> Handle(
        UpdateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (warehouse is null)
        {
            return null;
        }

        var organizationExists = await _context.Organizations
            .AnyAsync(o => o.Id == request.OrganizationId && !o.IsDeleted, cancellationToken);

        if (!organizationExists)
        {
            throw new NotFoundException("Organization not found.");
        }

        var addressExists = await _context.Addresses
            .AnyAsync(a => a.Id == request.AddressId, cancellationToken);

        if (!addressExists)
        {
            throw new NotFoundException("Address not found.");
        }

        var name = request.Name.Trim();

        var nameExists = await _context.Warehouses
            .AnyAsync(
                w => w.Id != request.Id
                     && w.OrganizationId == request.OrganizationId
                     && !w.IsDeleted
                     && w.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("A warehouse with this name already exists for the organization.");
        }

        warehouse.OrganizationId = request.OrganizationId;
        warehouse.AddressId = request.AddressId;
        warehouse.Name = name;
        warehouse.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Warehouse updated with Id {WarehouseId}", warehouse.Id);

        return WarehouseMapper.Map(warehouse);
    }
}
