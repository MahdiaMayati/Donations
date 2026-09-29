using Donation.Application.DTOs.Common;
using Donation.Application.DTOs.Organization.Response;
using MediatR;

namespace Donation.Application.Features.Organizations.Queries.GetAllOrganizations;

public sealed record GetAllOrganizationsQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PagedResult<OrganizationResponse>>;
