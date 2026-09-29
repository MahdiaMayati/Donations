using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Organizations.Commands.DeleteOrganization;

public sealed class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IOrganizationDependencyChecker _dependencyChecker;
    private readonly ILogger<DeleteOrganizationCommandHandler> _logger;

    public DeleteOrganizationCommandHandler(
        IAppDbContext context,
        IOrganizationDependencyChecker dependencyChecker,
        ILogger<DeleteOrganizationCommandHandler> logger)
    {
        _context = context;
        _dependencyChecker = dependencyChecker;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(
                o => o.Id == request.Id && !o.IsDeleted,
                cancellationToken);

        if (organization is null)
        {
            return false;
        }

        var (hasDependencies, message) = await _dependencyChecker.CheckAsync(request.Id, cancellationToken);
        if (hasDependencies)
        {
            throw new BusinessRuleException(
                string.IsNullOrWhiteSpace(message)
                    ? "Cannot delete this organization because it is referenced by related records."
                    : message);
        }

        organization.IsDeleted = true;
        organization.DeletedAt = DateTime.UtcNow;
        organization.IsActive = false;
        organization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Organization soft-deleted with Id {OrganizationId}", request.Id);

        return true;
    }
}
