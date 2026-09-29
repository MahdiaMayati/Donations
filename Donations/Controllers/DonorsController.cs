using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Donor.Request;
using Donation.Application.Features.Donors.Commands.CreateDonor;
using Donation.Application.Features.Donors.Commands.DeleteDonor;
using Donation.Application.Features.Donors.Commands.UpdateDonor;
using Donation.Application.Features.Donors.Queries.GetAllDonors;
using Donation.Application.Features.Donors.Queries.GetDonorById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DonorsController : BaseController
{
    private readonly ISender _sender;

    public DonorsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var donors = await _sender.Send(new GetAllDonorsQuery(), cancellationToken);
            return CustomResponse(donors, "Donors retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var donor = await _sender.Send(new GetDonorByIdQuery(id), cancellationToken);
            if (donor is null)
            {
                return CustomErrorResponse("Donor not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(donor, "Donor retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDonorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var donor = await _sender.Send(
                new CreateDonorCommand(
                    request.FullName,
                    request.Email,
                    request.PhoneNumber,
                    request.Password,
                    request.PreferredContactMethod,
                    request.Address),
                cancellationToken);

            return CustomResponse(donor, "Donor created successfully.", StatusCodes.Status201Created);
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
        catch (NotFoundException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status404NotFound);
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDonorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var donor = await _sender.Send(
                new UpdateDonorCommand(
                    id,
                    request.FullName,
                    request.Email,
                    request.PhoneNumber,
                    request.Password,
                    request.PreferredContactMethod,
                    request.Address),
                cancellationToken);

            if (donor is null)
            {
                return CustomErrorResponse("Donor not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(donor, "Donor updated successfully.");
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
        catch (NotFoundException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status404NotFound);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteDonorCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Donor not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Donor deleted successfully.");
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
