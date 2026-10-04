using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Warehouse.Response;
using MediatR;

namespace Donation.Application.Features.Warehouses.Queries.GetAllWarehouses;

public sealed record GetAllWarehousesQuery(
    Guid? OrganizationId = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<WarehouseResponse>>;
