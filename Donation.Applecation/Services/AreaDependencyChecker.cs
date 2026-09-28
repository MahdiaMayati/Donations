using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Services;

public sealed class AreaDependencyChecker : IAreaDependencyChecker
{
    private readonly IAppDbContext _context;

    public AreaDependencyChecker(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool HasDependencies, string Message)> CheckAsync(int areaId, CancellationToken cancellationToken)
    {
        var hasAddresses = await _context.Addresses
            .AnyAsync(a => a.AreaId == areaId, cancellationToken);

        if (hasAddresses)
        {
            return (true, "Cannot delete this area because it has related addresses.");
        }

        return (false, string.Empty);
    }
}
