using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Address.Request;
using Donation.Application.Features.Addresses.Commands.CreateAddress;
using Donation.Application.Features.Addresses.Commands.DeleteAddress;
using Donation.Application.Features.Addresses.Commands.UpdateAddress;
using Donation.Application.Features.Addresses.Queries.GetAddressById;
using Donation.Application.Features.Addresses.Queries.GetAllAddresses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressesController : BaseController
{
    private readonly ISender _sender;

    public AddressesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? areaId, CancellationToken cancellationToken)
    {
        try
        {
            var addresses = await _sender.Send(new GetAllAddressesQuery(areaId), cancellationToken);
            return CustomResponse(addresses, "Addresses retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var address = await _sender.Send(new GetAddressByIdQuery(id), cancellationToken);
            if (address is null)
            {
                return CustomErrorResponse("Address not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(address, "Address retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var address = await _sender.Send(
                new CreateAddressCommand(
                    request.AreaId,
                    request.Street,
                    request.Details,
                    request.Latitude,
                    request.Longitude),
                cancellationToken);

            return CustomResponse(address, "Address created successfully.", StatusCodes.Status201Created);
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
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var address = await _sender.Send(
                new UpdateAddressCommand(
                    id,
                    request.AreaId,
                    request.Street,
                    request.Details,
                    request.Latitude,
                    request.Longitude),
                cancellationToken);

            if (address is null)
            {
                return CustomErrorResponse("Address not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(address, "Address updated successfully.");
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
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteAddressCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Address not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Address deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }
}
