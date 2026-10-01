using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.User.Request;
using Donation.Application.Features.Users.Commands.UpdateUserProfile;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : BaseController
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Updates the authenticated user's profile.
    /// </summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Gender is null)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                new[] { new { PropertyName = nameof(request.Gender), ErrorMessage = "Gender is required (true = Male, false = Female)." } });
        }

        try
        {
            var result = await _sender.Send(
                new UpdateUserProfileCommand(
                    request.FirstName,
                    request.LastName,
                    request.DateOfBirth,
                    request.Gender.Value,
                    request.PreferredContactMethod,
                    request.MaritalStatus,
                    request.EducationalStatus,
                    request.Job,
                    request.HealthStatus),
                cancellationToken);

            return CustomResponse(result, "User profile updated successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        catch (NotFoundException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }
}
