using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.StorageLocation.Request;
using Donation.Application.Features.StorageLocations.Commands.CreateStorageLocation;
using Donation.Application.Features.StorageLocations.Commands.DeleteStorageLocation;
using Donation.Application.Features.StorageLocations.Commands.UpdateStorageLocation;
using Donation.Application.Features.StorageLocations.Queries.GetAllStorageLocations;
using Donation.Application.Features.StorageLocations.Queries.GetStorageLocationById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/storage-locations")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class StorageLocationsController : BaseController
{
    private readonly ISender _sender;

    public StorageLocationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lists storage locations with optional warehouse filter.
    /// Omit <paramref name="warehouseId"/> (or pass null/empty) to return all locations across warehouses.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? warehouseId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllStorageLocationsQuery(
                    NormalizeOptionalGuid(warehouseId),
                    pagination.Page,
                    pagination.Limit,
                    pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Storage locations retrieved successfully.");
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var location = await _sender.Send(new GetStorageLocationByIdQuery(id), cancellationToken);
            if (location is null)
            {
                return CustomErrorResponse("Storage location not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(location, "Storage location retrieved successfully.");
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
        [FromBody] CreateStorageLocationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var location = await _sender.Send(
                new CreateStorageLocationCommand(request.WarehouseId, request.Code),
                cancellationToken);

            return CustomResponse(location, "Storage location created successfully.", StatusCodes.Status201Created);
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
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateStorageLocationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var location = await _sender.Send(
                new UpdateStorageLocationCommand(id, request.WarehouseId, request.Code),
                cancellationToken);

            if (location is null)
            {
                return CustomErrorResponse("Storage location not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(location, "Storage location updated successfully.");
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
            var deleted = await _sender.Send(new DeleteStorageLocationCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Storage location not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Storage location deleted successfully.");
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
