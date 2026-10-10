using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.TaskType.Request;
using Donation.Application.Features.TaskTypes.Commands.CreateTaskType;
using Donation.Application.Features.TaskTypes.Commands.DeleteTaskType;
using Donation.Application.Features.TaskTypes.Commands.UpdateTaskType;
using Donation.Application.Features.TaskTypes.Queries.GetAllTaskTypes;
using Donation.Application.Features.TaskTypes.Queries.GetTaskTypeById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiExplorerSettings(GroupName = "tasks")]
[ApiController]
[Route("api/v1/task-types")]
[Authorize(Roles = "Admin,SuperAdmin")]
public sealed class TaskTypesController : BaseController
{
    private readonly ISender _sender;

    public TaskTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllTaskTypesQuery(pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Task types retrieved successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new GetTaskTypeByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Task type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Task type retrieved successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskTypeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateTaskTypeCommand(request.Code, request.Name),
                cancellationToken);
            return CustomResponse(item, "Task type created successfully.", StatusCodes.Status201Created);
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
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTaskTypeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateTaskTypeCommand(id, request.Code, request.Name),
                cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Task type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Task type updated successfully.");
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
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteTaskTypeCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Task type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Task type deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }
}
