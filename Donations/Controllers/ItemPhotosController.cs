using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.ItemPhoto.Request;
using Donation.Application.Features.ItemPhotos.Commands.CreateItemPhoto;
using Donation.Application.Features.ItemPhotos.Commands.DeleteItemPhoto;
using Donation.Application.Features.ItemPhotos.Commands.UpdateItemPhoto;
using Donation.Application.Features.ItemPhotos.Queries.GetAllItemPhotos;
using Donation.Application.Features.ItemPhotos.Queries.GetItemPhotoById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/item-photos")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ItemPhotosController : BaseController
{
    private readonly ISender _sender;

    public ItemPhotosController(ISender sender)
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
                new GetAllItemPhotosQuery(
                    NormalizeOptionalGuid(itemId),
                    pagination.Page,
                    pagination.Limit),
                cancellationToken);
            return CustomResponse(result, "Item photos retrieved successfully.");
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
            var photo = await _sender.Send(new GetItemPhotoByIdQuery(id), cancellationToken);
            if (photo is null)
            {
                return CustomErrorResponse("Item photo not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(photo, "Item photo retrieved successfully.");
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
        [FromBody] CreateItemPhotoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var photo = await _sender.Send(
                new CreateItemPhotoCommand(request.ItemId, request.Url),
                cancellationToken);
            return CustomResponse(photo, "Item photo created successfully.", StatusCodes.Status201Created);
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateItemPhotoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var photo = await _sender.Send(
                new UpdateItemPhotoCommand(id, request.Url),
                cancellationToken);
            if (photo is null)
            {
                return CustomErrorResponse("Item photo not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(photo, "Item photo updated successfully.");
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
            var deleted = await _sender.Send(new DeleteItemPhotoCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Item photo not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Item photo deleted successfully.");
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
