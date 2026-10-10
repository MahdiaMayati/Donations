using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Volunteer.Request;
using Donation.Application.Features.Volunteers.Commands.CreateVolunteer;
using Donation.Application.Features.Volunteers.Commands.DeleteVolunteer;
using Donation.Application.Features.Volunteers.Commands.RestoreVolunteer;
using Donation.Application.Features.Volunteers.Commands.UpdateVolunteer;
using Donation.Application.Features.Volunteers.Queries.GetAllVolunteers;
using Donation.Application.Features.Volunteers.Queries.GetDeletedVolunteers;
using Donation.Application.Features.Volunteers.Queries.GetDeletedVolunteersCount;
using Donation.Application.Features.Volunteers.Queries.GetVolunteerById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VolunteersController : BaseController
{
    private readonly ISender _sender;

    public VolunteersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(new GetAllVolunteersQuery(), cancellationToken);
            return CustomResponse(items, "Volunteers retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpGet("deleted/count")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetDeletedCount(CancellationToken cancellationToken)
    {
        try
        {
            var count = await _sender.Send(new GetDeletedVolunteersCountQuery(), cancellationToken);
            return CustomResponse(new { count }, "Soft-deleted volunteers count retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetDeleted(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(new GetDeletedVolunteersQuery(), cancellationToken);
            return CustomResponse(items, "Soft-deleted volunteers retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new GetVolunteerByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Volunteer not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Volunteer retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVolunteerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateVolunteerCommand(
                    request.OrganizationId,
                    request.Status,
                    request.Days,
                    request.HoursCount,
                    request.Hobbies,
                    request.Skills,
                    request.Experiences,
                    request.NeglectedTasksCount,
                    request.Address),
                cancellationToken);

            return CustomResponse(item, "Volunteer created successfully.", StatusCodes.Status201Created);
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
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

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new RestoreVolunteerCommand(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Soft-deleted volunteer not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Volunteer profile restored successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVolunteerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateVolunteerCommand(
                    id,
                    request.OrganizationId,
                    request.Status,
                    request.Days,
                    request.HoursCount,
                    request.Hobbies,
                    request.Skills,
                    request.Experiences,
                    request.NeglectedTasksCount,
                    request.Address),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("Volunteer not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Volunteer updated successfully.");
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
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteVolunteerCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Volunteer not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Volunteer deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }
}
