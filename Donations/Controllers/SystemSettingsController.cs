using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.SystemSetting.Request;
using Donation.Application.Features.SystemSettings.Commands.CreateSystemSetting;
using Donation.Application.Features.SystemSettings.Commands.DeleteSystemSetting;
using Donation.Application.Features.SystemSettings.Commands.UpdateSystemSetting;
using Donation.Application.Features.SystemSettings.Queries.GetAllSystemSettings;
using Donation.Application.Features.SystemSettings.Queries.GetSystemSettingById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiExplorerSettings(GroupName = "system")]
[ApiController]
[Route("api/v1/system-settings")]
public sealed class SystemSettingsController : BaseController
{
    private readonly ISender _sender;

    public SystemSettingsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Paginated list. Optional <paramref name="type"/> filters by setting type;
    /// when omitted, all settings are returned.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        [FromQuery] Guid? organizationId,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllSystemSettingsQuery(
                    type,
                    organizationId,
                    pagination.Page,
                    pagination.Limit,
                    pagination.Search),
                cancellationToken);
            return CustomResponse(result, "System settings retrieved successfully.");
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
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(new GetSystemSettingByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("System setting not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "System setting retrieved successfully.");
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateSystemSettingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateSystemSettingCommand(
                    request.OrganizationId,
                    request.Key,
                    request.Value,
                    request.Type),
                cancellationToken);
            return CustomResponse(item, "System setting created successfully.", StatusCodes.Status201Created);
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
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSystemSettingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateSystemSettingCommand(
                    id,
                    request.OrganizationId,
                    request.Key,
                    request.Value,
                    request.Type),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("System setting not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "System setting updated successfully.");
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
            var deleted = await _sender.Send(new DeleteSystemSettingCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("System setting not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "System setting deleted successfully.");
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
