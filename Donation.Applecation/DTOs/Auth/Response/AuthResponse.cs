namespace Donation.Application.DTOs.Auth.Response;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty; // أضيفي هذا السطر هنا
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}