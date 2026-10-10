using Donation.Application.DTOs.Warehouse.Response;
using MediatR;

namespace Donation.Application.Features.Warehouses.Commands.UpdateWarehouse;

public sealed record UpdateWarehouseCommand(
    Guid Id,
    Guid OrganizationId,
    Guid AddressId,
    string Name,
    bool IsActive) : IRequest<WarehouseResponse?>;
