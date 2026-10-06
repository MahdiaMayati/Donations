using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.DonationRequest.Request;
using Donation.Application.Features.DonationRequests.Commands.CreateDonationRequest;
using Donation.Application.Features.DonationRequests.Commands.DeleteDonationRequest;
using Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequest;
using Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequestStatus;
using Donation.Application.Features.DonationRequests.Queries.GetAllDonationRequests;
using Donation.Application.Features.DonationRequests.Queries.GetDonationRequestById;
using Donation.Domain.Enums;
using Donations.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/donation-requests")]
[Authorize]
public class DonationRequestsController : BaseController
{
    private readonly ISender _sender;

    public DonationRequestsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid organizationId,
        [FromQuery] Guid? donorId,
        [FromQuery] DonationRequestStatus? status,
        [FromQuery] DeliveryMethod? deliveryMethod,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllDonationRequestsQuery(
                    organizationId,
                    donorId,
                    status,
                    deliveryMethod,
                    pagination.Page,
                    pagination.Limit,
                    pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Donation requests retrieved successfully.");
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
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new GetDonationRequestByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Donation request not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Donation request retrieved successfully.");
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
    }

    [HttpPost]
    [EnableRateLimiting(RateLimitingPolicies.DonationSubmission)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDonationRequestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateDonationRequestCommand(
                    request.OrganizationId,
                    request.DonorId,
                    request.PickupAddressId,
                    request.DeliveryMethod,
                    request.Description,
                    request.EstimatedItemCount,
                    request.RequestPhotoUrls,
                    request.Items),
                cancellationToken);

            return CustomResponse(item, "Donation request created successfully.", StatusCodes.Status201Created);
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
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDonationRequestRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateDonationRequestCommand(
                    id,
                    request.PickupAddressId,
                    request.DeliveryMethod,
                    request.Description,
                    request.EstimatedItemCount,
                    request.RequestPhotoUrls,
                    request.Items),
                cancellationToken);

            return CustomResponse(item, "Donation request updated successfully.");
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
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateDonationRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateDonationRequestStatusCommand(id, request.Status),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("Donation request not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Donation request status updated successfully.");
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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteDonationRequestCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Donation request not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Donation request cancelled successfully.");
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
