using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Queries.GetDeletedMaterials;

public sealed record GetDeletedMaterialsQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<MaterialResponse>>;
