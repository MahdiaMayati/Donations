using Donation.Application.DTOs.StorageLocation.Response;
using MediatR;

namespace Donation.Application.Features.StorageLocations.Commands.UpdateStorageLocation;

public sealed record UpdateStorageLocationCommand(
    Guid Id,
    Guid WarehouseId,
    string Code) : IRequest<StorageLocationResponse?>;
