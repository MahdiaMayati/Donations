namespace Donation.Application.Abstractions.Services;

public interface IAddressDependencyChecker
{
    Task<(bool HasDependencies, string Message)> CheckAsync(Guid addressId, CancellationToken cancellationToken);
}
