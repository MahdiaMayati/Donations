using MediatR;

namespace Donation.Application.Features.StorageLocations.Commands.DeleteStorageLocation;

public sealed record DeleteStorageLocationCommand(Guid Id) : IRequest<bool>;
