using MediatR;

namespace Donation.Application.Features.Warehouses.Commands.DeleteWarehouse;

public sealed record DeleteWarehouseCommand(Guid Id) : IRequest<bool>;
