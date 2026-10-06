using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemStatusHistory.Request;
using Donation.Application.Features.ItemStatusHistories.Commands.CreateItemStatusHistory;
using Donation.Application.Features.ItemStatusHistories.Commands.DeleteItemStatusHistory;
using Donation.Application.Features.ItemStatusHistories.Queries.GetAllItemStatusHistories;
using Donation.Application.Features.ItemStatusHistories.Queries.GetItemStatusHistoryById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/item-status-histories")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ItemStatusHistoriesController : BaseController
{
    private readonly ISender _sender;

    public ItemStatusHistoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? itemId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllItemStatusHistoriesQuery(
                    NormalizeOptionalGuid(itemId),
                    pagination.Page,
                    pagination.Limit),
                cancellationToken);
            return CustomResponse(result, "Item status histories retrieved successfully.");
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
            var history = await _sender.Send(new GetItemStatusHistoryByIdQuery(id), cancellationToken);
            if (history is null)
            {
                return CustomErrorResponse("Item status history not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(history, "Item status history retrieved successfully.");
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
        [FromBody] CreateItemStatusHistoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var history = await _sender.Send(
                new CreateItemStatusHistoryCommand(
                    request.ItemId,
                    request.NewStatus,
                    request.OldStatus),
                cancellationToken);
            return CustomResponse(history, "Item status history created successfully.", StatusCodes.Status201Created);
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(
                new DeleteItemStatusHistoryCommand(id),
                cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item status history not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item status history deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    private static Guid? NormalizeOptionalGuid(Guid? value)
        => value is { } id && id != Guid.Empty ? id : null;
}
