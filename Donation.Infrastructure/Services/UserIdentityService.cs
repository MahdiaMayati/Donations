using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Donation.Infrastructure.Services;

public sealed class UserIdentityService : IUserIdentityService
{
    private readonly UserManager<User> _userManager;

    public UserIdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task EnsureEmailAvailableAsync(
        string email,
        Guid? excludeUserId,
        CancellationToken cancellationToken)
    {
        var existing = await _userManager.FindByEmailAsync(email.Trim());
        if (existing is null)
        {
            return;
        }

        if (excludeUserId.HasValue && existing.Id == excludeUserId.Value)
        {
            return;
        }

        throw new ConflictException("Email is already in use.");
    }

    public async Task ChangePasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessRuleException($"Failed to update password: {errors}");
        }
    }
}
