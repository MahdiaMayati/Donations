using Donation.Application.DTOs.StorageLocation.Response;
using MediatR;

namespace Donation.Application.Features.StorageLocations.Commands.CreateStorageLocation;

public sealed record CreateStorageLocationCommand(
    Guid WarehouseId,
    string Code) : IRequest<StorageLocationResponse>;
