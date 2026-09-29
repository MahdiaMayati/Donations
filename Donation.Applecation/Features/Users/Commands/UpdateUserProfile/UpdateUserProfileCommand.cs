using Donation.Application.DTOs.User.Response;
using MediatR;

namespace Donation.Application.Features.Users.Commands.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    bool Gender,
    string PreferredContactMethod,
    string MaritalStatus,
    string EducationalStatus,
    string Job,
    string HealthStatus) : IRequest<UserResponse>;
