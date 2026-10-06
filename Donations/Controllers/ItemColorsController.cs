using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemColor.Request;
using Donation.Application.Features.ItemColors.Commands.CreateItemColor;
using Donation.Application.Features.ItemColors.Commands.DeleteItemColor;
using Donation.Application.Features.ItemColors.Queries.GetAllItemColors;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/item-colors")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ItemColorsController : BaseController
{
    private readonly ISender _sender;

    public ItemColorsController(ISender sender)
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
                new GetAllItemColorsQuery(
                    NormalizeOptionalGuid(itemId),
                    pagination.Page,
                    pagination.Limit),
                cancellationToken);
            return CustomResponse(result, "Item colors retrieved successfully.");
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
        [FromBody] CreateItemColorRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var link = await _sender.Send(
                new CreateItemColorCommand(request.ItemId, request.ColorId),
                cancellationToken);
            return CustomResponse(link, "Item color linked successfully.", StatusCodes.Status201Created);
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

    [HttpDelete("{itemId:guid}/{colorId:guid}")]
    public async Task<IActionResult> Delete(
        Guid itemId,
        Guid colorId,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(
                new DeleteItemColorCommand(itemId, colorId),
                cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item color link not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item color unlinked successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteByQuery(
        [FromQuery] Guid itemId,
        [FromQuery] Guid colorId,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(
                new DeleteItemColorCommand(itemId, colorId),
                cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item color link not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item color unlinked successfully.");
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
