using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Donation.Application.Abstractions.Authentication;
using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.DTOs.Auth.Request;
using Donation.Application.DTOs.Auth.Response;
using Donation.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Donation.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly UserManager<User> _userManager;

    public AuthService(
        IAppDbContext context,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenProvider jwtTokenProvider,
        UserManager<User> userManager)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
        _userManager = userManager;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request.OrganizationId == Guid.Empty)
        {
            throw new Exception("OrganizationId is required.");
        }

        var organization = await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(
                o => o.Id == request.OrganizationId && !o.IsDeleted,
                cancellationToken);

        if (organization is null)
        {
            throw new Exception("Organization not found.");
        }

        if (!organization.IsActive)
        {
            throw new Exception("Organization is inactive.");
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new Exception("البريد الإلكتروني مستخدم مسبقاً.");
        }

        var (firstName, lastName) = SplitFullName(request.FullName);

        var user = new User
        {
            UserName = request.Email,
            FirstName = firstName,
            LastName = lastName,
            Email = request.Email.Trim(),
            IsActive = true,
            OrganizationId = request.OrganizationId,
            DateOfBirth = request.DateOfBirth?.Date,
            Gender = request.Gender!.Value,
            PreferredContactMethod = request.PreferredContactMethod.Trim(),
            MaritalStatus = request.MaritalStatus.Trim(),
            EducationalStatus = request.EducationalStatus.Trim(),
            Job = request.Job.Trim(),
            HealthStatus = request.HealthStatus.Trim()
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"فشل إنشاء الحساب: {errors}");
        }

        // توليد التوكن مباشرة بعد نجاح التسجيل
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenProvider.GenerateToken(user, roles, new List<string>());

        var refreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
            UserId = user.Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken, // إرسال الـ RefreshToken للعميل
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new Exception("البريد الإلكتروني أو كلمة المرور غير صحيحة.");
        }

        // جلب الصلاحيات الخاصة بالمستخدم لتمريرها للـ JwtTokenProvider
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenProvider.GenerateToken(user, roles, new List<string>());

        var refreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
            UserId = user.Id
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken, // إرسال الـ RefreshToken للعميل
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
        {
            throw new Exception("توكن غير صالح.");
        }

        var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

        if (!refreshToken.IsActive)
        {
            throw new Exception("التوكن منتهي الصلاحية أو تم إبطاله.");
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        // جلب الصلاحيات لتوليد الـ Access Token الجديد
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtTokenProvider.GenerateToken(user, roles, new List<string>());
        var newRefreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
            UserId = user.Id
        });

        await _context.SaveChangesAsync(default);

        return new AuthResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken, // إرسال التوكن الجديد للعميل
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = roles.ToList()
        };
    }

    public async Task<bool> LogoutAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .SingleOrDefaultAsync(t => t.Token == token);

        if (refreshToken == null || !refreshToken.IsActive)
        {
            return false;
        }

        refreshToken.RevokedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync(default);
        return true;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static (string FirstName, string LastName) SplitFullName(string fullName)
    {
        var trimmed = fullName.Trim();
        var parts = trimmed.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return (parts[0], parts[0]);
        }

        return (parts[0], parts[1]);
    }
}