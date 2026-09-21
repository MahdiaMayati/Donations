using Donation.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Donation.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher<User>
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }
}