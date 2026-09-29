using Donation.Application.DTOs.Organization.Response;
using MediatR;

namespace Donation.Application.Features.Organizations.Commands.CreateOrganization;

public sealed record CreateOrganizationCommand(string Name, bool IsActive = true) : IRequest<OrganizationResponse>;
