using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Area.Request;
using Donation.Application.Features.Areas.Commands.CreateArea;
using Donation.Application.Features.Areas.Commands.DeleteArea;
using Donation.Application.Features.Areas.Commands.UpdateArea;
using Donation.Application.Features.Areas.Queries.GetAllAreas;
using Donation.Application.Features.Areas.Queries.GetAreaById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AreasController : BaseController
{
    private readonly ISender _sender;

    public AreasController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? cityId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var areas = await _sender.Send(
            new GetAllAreasQuery(cityId, pagination.Page, pagination.Limit, pagination.Search),
            cancellationToken);
        return CustomResponse(areas, "Areas retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var area = await _sender.Send(new GetAreaByIdQuery(id), cancellationToken);
        if (area is null)
        {
            return CustomErrorResponse("Area not found.", StatusCodes.Status404NotFound);
        }

        return CustomResponse(area, "Area retrieved successfully.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateAreaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var area = await _sender.Send(new CreateAreaCommand(request.CityId, request.Name), cancellationToken);
            return CustomResponse(area, "Area created successfully.", StatusCodes.Status201Created);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAreaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var area = await _sender.Send(new UpdateAreaCommand(id, request.CityId, request.Name), cancellationToken);
            if (area is null)
            {
                return CustomErrorResponse("Area not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(area, "Area updated successfully.");
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
            var deleted = await _sender.Send(new DeleteAreaCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Area not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Area deleted successfully.");
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
}
