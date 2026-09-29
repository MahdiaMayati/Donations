using Donation.Application.DTOs.Organization.Response;
using MediatR;

namespace Donation.Application.Features.Organizations.Queries.GetOrganizationById;

public sealed record GetOrganizationByIdQuery(Guid Id) : IRequest<OrganizationResponse?>;
