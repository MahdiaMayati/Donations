using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.City.Request;
using Donation.Application.Features.Cities.Commands.CreateCity;
using Donation.Application.Features.Cities.Commands.DeleteCity;
using Donation.Application.Features.Cities.Commands.UpdateCity;
using Donation.Application.Features.Cities.Queries.GetAllCities;
using Donation.Application.Features.Cities.Queries.GetCityById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : BaseController
{
    private readonly ISender _sender;

    public CitiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cities = await _sender.Send(new GetAllCitiesQuery(), cancellationToken);
        return CustomResponse(cities, "Cities retrieved successfully.");
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var city = await _sender.Send(new GetCityByIdQuery(id), cancellationToken);
        if (city is null)
        {
            return CustomErrorResponse("City not found.", StatusCodes.Status404NotFound);
        }

        return CustomResponse(city, "City retrieved successfully.");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateCityRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var city = await _sender.Send(new CreateCityCommand(request.Name, request.Code), cancellationToken);
            return CustomResponse(city, "City created successfully.", StatusCodes.Status201Created);
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
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var city = await _sender.Send(new UpdateCityCommand(id, request.Name, request.Code), cancellationToken);
            if (city is null)
            {
                return CustomErrorResponse("City not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(city, "City updated successfully.");
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
            var deleted = await _sender.Send(new DeleteCityCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("City not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "City deleted successfully.");
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
