using Donation.Application.Abstractions.Persistence;
using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.User.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Donation.Application.Features.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, UserResponse>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IAppDbContext context,
        ICurrentUserService currentUser,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenException("You must be authenticated to update your profile.");
        }

        var userId = _currentUser.UserId.Value;
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.DateOfBirth = request.DateOfBirth?.Date;
        user.Gender = request.Gender;
        user.PreferredContactMethod = request.PreferredContactMethod.Trim();
        user.MaritalStatus = request.MaritalStatus.Trim();
        user.EducationalStatus = request.EducationalStatus.Trim();
        user.Job = request.Job.Trim();
        user.HealthStatus = request.HealthStatus.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User profile updated for UserId {UserId}", userId);

        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            OrganizationId = user.OrganizationId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            PreferredContactMethod = user.PreferredContactMethod,
            MaritalStatus = user.MaritalStatus,
            EducationalStatus = user.EducationalStatus,
            Job = user.Job,
            HealthStatus = user.HealthStatus
        };
    }
}
