using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Auth.Request;
using Donation.Application.DTOs.Auth.Response;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthController(IAuthService authService, IValidator<RegisterRequest> registerValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Validation failed.",
                Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }),
                Data = (object?)null
            });
        }

        var result = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
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
