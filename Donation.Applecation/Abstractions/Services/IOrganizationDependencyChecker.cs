namespace Donation.Application.Abstractions.Services;

public interface IOrganizationDependencyChecker
{
    /// <summary>
    /// Returns whether the organization is referenced by users or roles that block soft deletion.
    /// </summary>
    Task<(bool HasDependencies, string Message)> CheckAsync(Guid organizationId, CancellationToken cancellationToken);
}
