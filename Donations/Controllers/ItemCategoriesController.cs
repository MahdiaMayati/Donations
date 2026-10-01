using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemCategory.Request;
using Donation.Application.Features.ItemCategories.Commands.CreateItemCategory;
using Donation.Application.Features.ItemCategories.Commands.DeleteItemCategory;
using Donation.Application.Features.ItemCategories.Commands.RestoreItemCategory;
using Donation.Application.Features.ItemCategories.Commands.UpdateItemCategory;
using Donation.Application.Features.ItemCategories.Queries.GetAllItemCategories;
using Donation.Application.Features.ItemCategories.Queries.GetDeletedItemCategories;
using Donation.Application.Features.ItemCategories.Queries.GetItemCategoryById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/item-categories")]
public sealed class ItemCategoriesController : BaseController
{
    private readonly ISender _sender;

    public ItemCategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetAllItemCategoriesQuery(pagination.Page, pagination.Limit, pagination.Search),
            cancellationToken);
        return CustomResponse(result, "Item categories retrieved successfully.");
    }

    [HttpGet("deleted")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetDeleted(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetDeletedItemCategoriesQuery(pagination.Page, pagination.Limit, pagination.Search),
            cancellationToken);
        return CustomResponse(result, "Soft-deleted item categories retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new GetItemCategoryByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Item category not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item category retrieved successfully.");
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
        [FromBody] CreateItemCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new CreateItemCategoryCommand(request.Name), cancellationToken);
            return CustomResponse(item, "Item category created successfully.", StatusCodes.Status201Created);
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateItemCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new UpdateItemCategoryCommand(id, request.Name), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Item category not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item category updated successfully.");
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteItemCategoryCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item category not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item category deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{id:guid}/restore")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new RestoreItemCategoryCommand(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Soft-deleted item category not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Item category restored successfully.");
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
}
