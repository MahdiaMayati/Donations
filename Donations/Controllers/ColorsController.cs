using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Color.Request;
using Donation.Application.Features.Colors.Commands.CreateColor;
using Donation.Application.Features.Colors.Commands.CreateColorsBulk;
using Donation.Application.Features.Colors.Commands.DeleteColor;
using Donation.Application.Features.Colors.Commands.DeleteColorsBulk;
using Donation.Application.Features.Colors.Commands.RestoreColor;
using Donation.Application.Features.Colors.Commands.RestoreColorsBulk;
using Donation.Application.Features.Colors.Commands.UpdateColor;
using Donation.Application.Features.Colors.Commands.UpdateColorsBulk;
using Donation.Application.Features.Colors.Queries.GetAllColors;
using Donation.Application.Features.Colors.Queries.GetColorById;
using Donation.Application.Features.Colors.Queries.GetDeletedColors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/colors")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ColorsController : BaseController
{
    private readonly ISender _sender;

    public ColorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] List<Guid>? ids,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllColorsQuery(ids, pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Colors retrieved successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpGet("batch")]
    public async Task<IActionResult> GetBatch(
        [FromQuery] List<Guid> ids,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (ids is null || ids.Count == 0)
            {
                return CustomErrorResponse("At least one id is required.", StatusCodes.Status400BadRequest);
            }

            var result = await _sender.Send(
                new GetAllColorsQuery(ids, pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Colors retrieved successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeleted(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetDeletedColorsQuery(pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Deleted colors retrieved successfully.");
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
            var color = await _sender.Send(new GetColorByIdQuery(id), cancellationToken);
            if (color is null)
            {
                return CustomErrorResponse("Color not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(color, "Color retrieved successfully.");
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
        [FromBody] CreateColorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await _sender.Send(
                new CreateColorCommand(request.Name, request.Code),
                cancellationToken);
            return CustomResponse(color, "Color created successfully.", StatusCodes.Status201Created);
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

    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch(
        [FromBody] List<CreateColorRequest> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var colors = await _sender.Send(
                new CreateColorsBulkCommand(
                    (request ?? new List<CreateColorRequest>())
                        .Select(r => new CreateColorItem(r.Name, r.Code))
                        .ToList()),
                cancellationToken);
            return CustomResponse(colors, "Colors created successfully.", StatusCodes.Status201Created);
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
        [FromBody] UpdateColorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var color = await _sender.Send(
                new UpdateColorCommand(id, request.Name, request.Code),
                cancellationToken);
            if (color is null)
            {
                return CustomErrorResponse("Color not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(color, "Color updated successfully.");
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

    [HttpPut("batch")]
    public async Task<IActionResult> UpdateBatch(
        [FromBody] List<UpdateColorRequest> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var body = request ?? new List<UpdateColorRequest>();
            if (body.Any(r => !r.Id.HasValue || r.Id.Value == Guid.Empty))
            {
                return CustomErrorResponse(
                    "Each item must include a valid Id for bulk update.",
                    StatusCodes.Status400BadRequest);
            }

            var colors = await _sender.Send(
                new UpdateColorsBulkCommand(
                    body.Select(r => new UpdateColorItem(r.Id!.Value, r.Name, r.Code)).ToList()),
                cancellationToken);
            return CustomResponse(colors, "Colors updated successfully.");
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
            var deleted = await _sender.Send(new DeleteColorCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Color not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Color deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteBatch(
        [FromBody] BulkColorIdsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = await _sender.Send(
                new DeleteColorsBulkCommand(request.Ids ?? new List<Guid>()),
                cancellationToken);
            return CustomResponse(new { deletedCount = count }, "Colors deleted successfully.");
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
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var color = await _sender.Send(new RestoreColorCommand(id), cancellationToken);
            if (color is null)
            {
                return CustomErrorResponse("Deleted color not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(color, "Color restored successfully.");
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

    [HttpPost("restore/batch")]
    public async Task<IActionResult> RestoreBatch(
        [FromBody] BulkColorIdsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var colors = await _sender.Send(
                new RestoreColorsBulkCommand(request.Ids ?? new List<Guid>()),
                cancellationToken);
            return CustomResponse(colors, "Colors restored successfully.");
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
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
    }
}
