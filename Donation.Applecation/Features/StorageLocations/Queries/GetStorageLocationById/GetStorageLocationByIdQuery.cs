using Donation.Application.DTOs.StorageLocation.Response;
using MediatR;

namespace Donation.Application.Features.StorageLocations.Queries.GetStorageLocationById;

public sealed record GetStorageLocationByIdQuery(Guid Id) : IRequest<StorageLocationResponse?>;
