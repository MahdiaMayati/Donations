using Donation.Application.DTOs.Warehouse.Response;
using MediatR;

namespace Donation.Application.Features.Warehouses.Queries.GetWarehouseById;

public sealed record GetWarehouseByIdQuery(Guid Id) : IRequest<WarehouseResponse?>;
