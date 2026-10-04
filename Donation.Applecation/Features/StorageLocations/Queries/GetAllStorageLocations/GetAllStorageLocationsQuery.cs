using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.StorageLocation.Response;
using MediatR;

namespace Donation.Application.Features.StorageLocations.Queries.GetAllStorageLocations;

public sealed record GetAllStorageLocationsQuery(
    Guid? WarehouseId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<StorageLocationResponse>>;
