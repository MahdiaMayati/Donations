using Donation.Application.DTOs.Organization.Response;
using MediatR;

namespace Donation.Application.Features.Organizations.Commands.UpdateOrganization;

public sealed record UpdateOrganizationCommand(Guid Id, string Name, bool IsActive)
    : IRequest<OrganizationResponse?>;
