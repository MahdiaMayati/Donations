using System.Security.Claims;
using Donation.Application.Abstractions.Services;
using Microsoft.AspNetCore.Http;

namespace Donation.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return null;
            }

            var raw =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value;

            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public bool IsAdmin
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return false;
            }

            return user.IsInRole("Admin") || user.IsInRole("SuperAdmin");
        }
    }
}
