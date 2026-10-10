using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemType.Request;
using Donation.Application.Features.ItemTypes.Commands.CreateItemType;
using Donation.Application.Features.ItemTypes.Commands.DeleteItemType;
using Donation.Application.Features.ItemTypes.Commands.RestoreItemType;
using Donation.Application.Features.ItemTypes.Commands.UpdateItemType;
using Donation.Application.Features.ItemTypes.Queries.GetAllItemTypes;
using Donation.Application.Features.ItemTypes.Queries.GetDeletedItemTypes;
using Donation.Application.Features.ItemTypes.Queries.GetItemTypeById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiExplorerSettings(GroupName = "catalog")]
[ApiController]
[Route("api/v1/item-types")]
public sealed class ItemTypesController : BaseController
{
    private readonly ISender _sender;

    public ItemTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? categoryId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllItemTypesQuery(categoryId, pagination.Page, pagination.Limit, pagination.Search),
            cancellationToken);
        return CustomResponse(result, "Item types retrieved successfully.");
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetDeleted(
        [FromQuery] Guid? categoryId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetDeletedItemTypesQuery(categoryId, pagination.Page, pagination.Limit, pagination.Search),
            cancellationToken);
        return CustomResponse(result, "Soft-deleted item types retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new GetItemTypeByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Item type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item type retrieved successfully.");
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemTypeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateItemTypeCommand(request.CategoryId, request.Name, request.OutfitUnits),
                cancellationToken);
            return CustomResponse(item, "Item type created successfully.", StatusCodes.Status201Created);
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

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateItemTypeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateItemTypeCommand(id, request.CategoryId, request.Name, request.OutfitUnits),
                cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Item type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item type updated successfully.");
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteItemTypeCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item type deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new RestoreItemTypeCommand(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Soft-deleted item type not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item type restored successfully.");
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
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }
}
