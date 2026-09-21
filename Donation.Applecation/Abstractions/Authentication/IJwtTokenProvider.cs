using Donation.Domain.Entities;

namespace Donation.Application.Abstractions.Authentication;

public interface IJwtTokenProvider
{
    string GenerateToken(User user, IList<string> roles, IList<string> permissions);
}