namespace Donation.Application.Abstractions.Services;

public interface IAreaDependencyChecker
{
    Task<(bool HasDependencies, string Message)> CheckAsync(Guid areaId, CancellationToken cancellationToken);
}
