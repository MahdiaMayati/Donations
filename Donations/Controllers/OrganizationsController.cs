using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Organization.Request;
using Donation.Application.Features.Organizations.Commands.CreateOrganization;
using Donation.Application.Features.Organizations.Commands.DeleteOrganization;
using Donation.Application.Features.Organizations.Commands.UpdateOrganization;
using Donation.Application.Features.Organizations.Queries.GetAllOrganizations;
using Donation.Application.Features.Organizations.Queries.GetOrganizationById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/organizations")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class OrganizationsController : BaseController
{
    private readonly ISender _sender;

    public OrganizationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllOrganizationsQuery(pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Organizations retrieved successfully.");
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
            var organization = await _sender.Send(new GetOrganizationByIdQuery(id), cancellationToken);
            if (organization is null)
            {
                return CustomErrorResponse("Organization not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(organization, "Organization retrieved successfully.");
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
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _sender.Send(
                new CreateOrganizationCommand(request.Name, request.IsActive),
                cancellationToken);
            return CustomResponse(organization, "Organization created successfully.", StatusCodes.Status201Created);
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
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _sender.Send(
                new UpdateOrganizationCommand(id, request.Name, request.IsActive),
                cancellationToken);
            if (organization is null)
            {
                return CustomErrorResponse("Organization not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(organization, "Organization updated successfully.");
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
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteOrganizationCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Organization not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Organization deleted successfully.");
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
