using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.Beneficiary.Request;
using Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;
using Donation.Application.Features.Beneficiaries.Commands.DeleteBeneficiary;
using Donation.Application.Features.Beneficiaries.Commands.UpdateBeneficiary;
using Donation.Application.Features.Beneficiaries.Queries.GetAllBeneficiaries;
using Donation.Application.Features.Beneficiaries.Queries.GetBeneficiaryById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BeneficiariesController : BaseController
{
    private readonly ISender _sender;

    public BeneficiariesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(new GetAllBeneficiariesQuery(), cancellationToken);
            return CustomResponse(items, "Beneficiaries retrieved successfully.");
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
            var item = await _sender.Send(new GetBeneficiaryByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Beneficiary not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Beneficiary retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBeneficiaryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateBeneficiaryCommand(
                    request.User,
                    request.City,
                    request.Area,
                    request.Address,
                    request.IdPhotoUrl,
                    request.IsHeadOfHousehold),
                cancellationToken);

            return CustomResponse(item, "Beneficiary created successfully.", StatusCodes.Status201Created);
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
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
        catch (NotFoundException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status404NotFound);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBeneficiaryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateBeneficiaryCommand(
                    id,
                    request.AddressId,
                    request.IdPhotoUrl,
                    request.IsHeadOfHousehold,
                    request.VerificationStatus,
                    request.VerifiedUntil),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("Beneficiary not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Beneficiary updated successfully.");
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
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
        catch (NotFoundException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status404NotFound);
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
            var deleted = await _sender.Send(new DeleteBeneficiaryCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Beneficiary not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Beneficiary deleted successfully.");
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
