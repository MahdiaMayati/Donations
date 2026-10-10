using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.FamilyMember.Request;
using Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMember;
using Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMembersBulk;
using Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;
using Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMembersBulk;
using Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMember;
using Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMembersBulk;
using Donation.Application.Features.FamilyMembers.Queries.GetAllFamilyMembers;
using Donation.Application.Features.FamilyMembers.Queries.GetFamilyMemberById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiExplorerSettings(GroupName = "beneficiaries")]
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class FamilyMembersController : BaseController
{
    private readonly ISender _sender;

    public FamilyMembersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>List family members (paginated). Optional: headOfHouseholdId, ids (batch).</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? headOfHouseholdId,
        [FromQuery] List<Guid>? ids,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(
                new GetAllFamilyMembersQuery(
                    headOfHouseholdId,
                    ids,
                    pagination.Page,
                    pagination.Limit,
                    pagination.Search),
                cancellationToken);
            return CustomResponse(items, "Family members retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    /// <summary>Batch get by ids.</summary>
    [HttpGet("batch")]
    public async Task<IActionResult> GetBatch(
        [FromQuery] List<Guid> ids,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        try
        {
            if (ids is null || ids.Count == 0)
            {
                return CustomErrorResponse("At least one id is required.", StatusCodes.Status400BadRequest);
            }

            var items = await _sender.Send(
                new GetAllFamilyMembersQuery(
                    Ids: ids,
                    Page: pagination.Page,
                    Limit: pagination.Limit,
                    Search: pagination.Search),
                cancellationToken);
            return CustomResponse(items, "Family members retrieved successfully.");
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
            var item = await _sender.Send(new GetFamilyMemberByIdQuery(id), cancellationToken);
            if (item is null)
            {
                return CustomErrorResponse("Family member not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Family member retrieved successfully.");
        }
        catch (ForbiddenException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status403Forbidden);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFamilyMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new CreateFamilyMemberCommand(
                    request.HeadOfHouseholdId,
                    request.FullName,
                    request.DateOfBirth,
                    request.Gender,
                    request.ClothingSize,
                    request.ShoeSize),
                cancellationToken);

            return CustomResponse(item, "Family member created successfully.", StatusCodes.Status201Created);
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
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch(
        [FromBody] List<CreateFamilyMemberRequest> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(
                new CreateFamilyMembersBulkCommand(
                    (request ?? new List<CreateFamilyMemberRequest>())
                        .Select(r => new CreateFamilyMemberItem(
                            r.HeadOfHouseholdId,
                            r.FullName,
                            r.DateOfBirth,
                            r.Gender,
                            r.ClothingSize,
                            r.ShoeSize))
                        .ToList()),
                cancellationToken);

            return CustomResponse(items, "Family members created successfully.", StatusCodes.Status201Created);
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
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateFamilyMemberRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateFamilyMemberCommand(
                    id,
                    request.HeadOfHouseholdId,
                    request.FullName,
                    request.DateOfBirth,
                    request.Gender,
                    request.ClothingSize,
                    request.ShoeSize),
                cancellationToken);

            if (item is null)
            {
                return CustomErrorResponse("Family member not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(item, "Family member updated successfully.");
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
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
        }
        catch (BusinessRuleException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("batch")]
    public async Task<IActionResult> UpdateBatch(
        [FromBody] List<UpdateFamilyMemberRequest> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var body = request ?? new List<UpdateFamilyMemberRequest>();
            if (body.Any(r => !r.Id.HasValue || r.Id.Value == Guid.Empty))
            {
                return CustomErrorResponse(
                    "Each item must include a valid Id for bulk update.",
                    StatusCodes.Status400BadRequest);
            }

            var items = await _sender.Send(
                new UpdateFamilyMembersBulkCommand(
                    body.Select(r => new UpdateFamilyMemberItem(
                            r.Id!.Value,
                            r.HeadOfHouseholdId,
                            r.FullName,
                            r.DateOfBirth,
                            r.Gender,
                            r.ClothingSize,
                            r.ShoeSize))
                        .ToList()),
                cancellationToken);

            return CustomResponse(items, "Family members updated successfully.");
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
        catch (ConflictException ex)
        {
            return CustomErrorResponse(ex.Message, StatusCodes.Status409Conflict);
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
            var deleted = await _sender.Send(new DeleteFamilyMemberCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Family member not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Family member deleted successfully.");
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

    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteBatch(
        [FromBody] BulkDeleteFamilyMembersRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = await _sender.Send(
                new DeleteFamilyMembersBulkCommand(request.Ids ?? new List<Guid>()),
                cancellationToken);

            return CustomResponse(new { deletedCount = count }, "Family members deleted successfully.");
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
    }
}
