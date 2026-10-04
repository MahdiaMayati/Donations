using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Queries.GetAllMaterials;

public sealed record GetAllMaterialsQuery(
    IReadOnlyList<Guid>? Ids = null,
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<MaterialResponse>>;
