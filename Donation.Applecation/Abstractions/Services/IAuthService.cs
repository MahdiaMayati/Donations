using Donation.Application.DTOs.Auth.Request;
using Donation.Application.DTOs.Auth.Response;

namespace Donation.Application.Abstractions.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> RefreshTokenAsync(string token);
    Task<bool> LogoutAsync(string token);
}