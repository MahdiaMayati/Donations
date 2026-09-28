namespace Donation.Application.Abstractions.Services;

public interface IAreaDependencyChecker
{
    Task<(bool HasDependencies, string Message)> CheckAsync(int areaId, CancellationToken cancellationToken);
}
