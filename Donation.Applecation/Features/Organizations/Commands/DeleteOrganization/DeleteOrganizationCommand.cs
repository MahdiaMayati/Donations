using MediatR;

namespace Donation.Application.Features.Organizations.Commands.DeleteOrganization;

public sealed record DeleteOrganizationCommand(Guid Id) : IRequest<bool>;
