using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Warehouse.Request;
using Donation.Application.Features.Warehouses.Commands.CreateWarehouse;
using Donation.Application.Features.Warehouses.Commands.DeleteWarehouse;
using Donation.Application.Features.Warehouses.Commands.UpdateWarehouse;
using Donation.Application.Features.Warehouses.Queries.GetAllWarehouses;
using Donation.Application.Features.Warehouses.Queries.GetWarehouseById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/warehouses")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class WarehousesController : BaseController
{
    private readonly ISender _sender;

    public WarehousesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lists warehouses with optional organization filter.
    /// Omit <paramref name="organizationId"/> (or pass null/empty) to return all non-deleted warehouses.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? organizationId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllWarehousesQuery(
                    NormalizeOptionalGuid(organizationId),
                    pagination.Page,
                    pagination.Limit,
                    pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Warehouses retrieved successfully.");
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
            var warehouse = await _sender.Send(new GetWarehouseByIdQuery(id), cancellationToken);
            if (warehouse is null)
            {
                return CustomErrorResponse("Warehouse not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(warehouse, "Warehouse retrieved successfully.");
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
        [FromBody] CreateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await _sender.Send(
                new CreateWarehouseCommand(
                    request.OrganizationId,
                    request.AddressId,
                    request.Name,
                    request.IsActive),
                cancellationToken);

            return CustomResponse(warehouse, "Warehouse created successfully.", StatusCodes.Status201Created);
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
        [FromBody] UpdateWarehouseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var warehouse = await _sender.Send(
                new UpdateWarehouseCommand(
                    id,
                    request.OrganizationId,
                    request.AddressId,
                    request.Name,
                    request.IsActive),
                cancellationToken);

            if (warehouse is null)
            {
                return CustomErrorResponse("Warehouse not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(warehouse, "Warehouse updated successfully.");
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
            var deleted = await _sender.Send(new DeleteWarehouseCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Warehouse not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Warehouse deleted successfully.");
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
