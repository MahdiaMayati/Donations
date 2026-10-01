using Donation.Application.DTOs.Warehouse.Response;
using MediatR;

namespace Donation.Application.Features.Warehouses.Commands.CreateWarehouse;

public sealed record CreateWarehouseCommand(
    Guid OrganizationId,
    Guid AddressId,
    string Name,
    bool IsActive = true) : IRequest<WarehouseResponse>;
