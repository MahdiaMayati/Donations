namespace Donation.Application.Abstractions.Services;

public interface ICityDependencyChecker
{
    /// <summary>
    /// Returns whether the city is referenced by dependent records that block hard deletion.
    /// </summary>
    Task<(bool HasDependencies, string Message)> CheckAsync(Guid cityId, CancellationToken cancellationToken);
}
