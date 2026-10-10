using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Beneficiary.Request;
using Donation.Application.Features.Beneficiaries.Commands.CreateBeneficiary;
using Donation.Application.Features.Beneficiaries.Commands.DeleteBeneficiary;
using Donation.Application.Features.Beneficiaries.Commands.RestoreBeneficiary;
using Donation.Application.Features.Beneficiaries.Commands.UpdateBeneficiary;
using Donation.Application.Features.Beneficiaries.Queries.GetAllBeneficiaries;
using Donation.Application.Features.Beneficiaries.Queries.GetBeneficiaryById;
using Donation.Application.Features.Beneficiaries.Queries.GetDeletedBeneficiaries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiExplorerSettings(GroupName = "beneficiaries")]
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BeneficiariesController : BaseController
{
    private readonly ISender _sender;

    public BeneficiariesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(
                new GetAllBeneficiariesQuery(pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(items, "Beneficiaries retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeleted(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(
                new GetDeletedBeneficiariesQuery(pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(items, "Deleted beneficiaries retrieved successfully.");
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

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new RestoreBeneficiaryCommand(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Deleted beneficiary not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Beneficiary restored successfully.");
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBeneficiaryRequest request, CancellationToken cancellationToken)
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
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
