namespace Donation.Application.Abstractions.Services;

public interface IUserIdentityService
{
    Task EnsureEmailAvailableAsync(string email, Guid? excludeUserId, CancellationToken cancellationToken);

    Task ChangePasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken);
}
