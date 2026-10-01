using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Organization.Response;
using MediatR;

namespace Donation.Application.Features.Organizations.Queries.GetAllOrganizations;

public sealed record GetAllOrganizationsQuery(
    int Page = PaginationRequest.DefaultPage,
    int Limit = PaginationRequest.DefaultLimit,
    string? Search = null) : IRequest<PaginatedResult<OrganizationResponse>>;
