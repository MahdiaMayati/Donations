using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Services;

public sealed class OrganizationDependencyChecker : IOrganizationDependencyChecker
{
    private readonly IAppDbContext _context;

    public OrganizationDependencyChecker(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool HasDependencies, string Message)> CheckAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var hasUsers = await _context.Users
            .AnyAsync(u => u.OrganizationId == organizationId, cancellationToken);

        if (hasUsers)
        {
            return (true, "Cannot delete this organization because it has related users.");
        }

        var hasRoles = await _context.Roles
            .AnyAsync(r => r.OrganizationId == organizationId && !r.IsDeleted, cancellationToken);

        if (hasRoles)
        {
            return (true, "Cannot delete this organization because it has related roles.");
        }

        var hasWarehouses = await _context.Warehouses
            .AnyAsync(w => w.OrganizationId == organizationId && !w.IsDeleted, cancellationToken);

        if (hasWarehouses)
        {
            return (true, "Cannot delete this organization because it has related warehouses.");
        }

        return (false, string.Empty);
    }
}
