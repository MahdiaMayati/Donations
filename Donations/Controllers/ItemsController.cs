using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Item.Request;
using Donation.Application.Features.Items.Commands.CreateItem;
using Donation.Application.Features.Items.Commands.DeleteItem;
using Donation.Application.Features.Items.Commands.UpdateItem;
using Donation.Application.Features.Items.Commands.UpdateItemSortingStatus;
using Donation.Application.Features.Items.Queries.GetAllItems;
using Donation.Application.Features.Items.Queries.GetItemById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/items")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ItemsController : BaseController
{
    private readonly ISender _sender;

    public ItemsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? organizationId,
        [FromQuery] Guid? donationRequestId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllItemsQuery(
                    NormalizeOptionalGuid(organizationId),
                    NormalizeOptionalGuid(donationRequestId),
                    pagination.Page,
                    pagination.Limit,
                    pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Items retrieved successfully.");
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
            var item = await _sender.Send(new GetItemByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Item not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item retrieved successfully.");
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
        [FromBody] CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateItemCommand(
                    request.OrganizationId,
                    request.DonationRequestId,
                    request.ItemTypeId,
                    request.MaterialId,
                    request.StorageLocationId,
                    request.SortedByUserId,
                    request.Barcode,
                    request.TargetGender,
                    request.AgeGroup,
                    request.Size,
                    request.Season,
                    request.Condition,
                    request.AvailabilityStatus,
                    request.SorterNotes,
                    request.ReceivedAt),
                cancellationToken);
            return CustomResponse(item, "Item created successfully.", StatusCodes.Status201Created);
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
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateItemCommand(
                    id,
                    request.ItemTypeId,
                    request.MaterialId,
                    request.StorageLocationId,
                    request.SortedByUserId,
                    request.Barcode,
                    request.TargetGender,
                    request.AgeGroup,
                    request.Size,
                    request.Season,
                    request.Condition,
                    request.SortingStatus,
                    request.AvailabilityStatus,
                    request.SorterNotes,
                    request.ReceivedAt),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("Item not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item updated successfully.");
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

    [HttpPatch("{id:guid}/sorting-status")]
    public async Task<IActionResult> UpdateSortingStatus(
        Guid id,
        [FromBody] UpdateItemSortingStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateItemSortingStatusCommand(id, request.SortingStatus),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("Item not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item sorting status updated successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteItemCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item deleted successfully.");
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
