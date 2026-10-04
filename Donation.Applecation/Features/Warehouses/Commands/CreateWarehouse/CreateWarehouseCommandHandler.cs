using Donation.Application.Abstractions.Persistence;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Warehouse.Response;
using Donation.Application.Features.Warehouses.Common;
using Donation.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Warehouses.Commands.CreateWarehouse;

public sealed class CreateWarehouseCommandHandler
    : IRequestHandler<CreateWarehouseCommand, WarehouseResponse>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<CreateWarehouseCommandHandler> _logger;

    public CreateWarehouseCommandHandler(
        IAppDbContext context,
        ILogger<CreateWarehouseCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WarehouseResponse> Handle(
        CreateWarehouseCommand request,
        CancellationToken cancellationToken)
    {
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
                w => w.OrganizationId == request.OrganizationId
                     && !w.IsDeleted
                     && w.Name.ToLower() == name.ToLower(),
                cancellationToken);

        if (nameExists)
        {
            throw new ConflictException("A warehouse with this name already exists for the organization.");
        }

        var warehouse = new Warehouse
        {
            OrganizationId = request.OrganizationId,
            AddressId = request.AddressId,
            Name = name,
            IsActive = request.IsActive,
            IsDeleted = false
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Warehouse created with Id {WarehouseId}", warehouse.Id);

        return WarehouseMapper.Map(warehouse);
    }
}
