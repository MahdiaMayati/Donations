using Donation.Application.Abstractions.Services;

namespace Donation.Application.Services;

/// <summary>
/// Donation / operational links to Address are not modeled yet.
/// Extend this when donations or other active operations reference AddressId.
/// </summary>
public sealed class AddressDependencyChecker : IAddressDependencyChecker
{
    public Task<(bool HasDependencies, string Message)> CheckAsync(int addressId, CancellationToken cancellationToken)
    {
        return Task.FromResult((false, string.Empty));
    }
}
