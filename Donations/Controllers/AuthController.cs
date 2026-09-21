using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Auth.Request;
using Donation.Application.DTOs.Auth.Response;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        return Ok(result);
    }


    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] string token)
    {
        var result = await _authService.RefreshTokenAsync(token);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<bool>> Logout([FromBody] string token)
    {
        var result = await _authService.LogoutAsync(token);
        return Ok(result);
    }
}