using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Services;

public sealed class CityDependencyChecker : ICityDependencyChecker
{
    private readonly IAppDbContext _context;

    public CityDependencyChecker(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool HasDependencies, string Message)> CheckAsync(int cityId, CancellationToken cancellationToken)
    {
        var hasAreas = await _context.Areas
            .AnyAsync(a => a.CityId == cityId, cancellationToken);

        if (hasAreas)
        {
            return (true, "Cannot delete this city because it has related areas.");
        }

        return (false, string.Empty);
    }
}
