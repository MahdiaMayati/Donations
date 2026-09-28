using Donation.Application.Common.Exceptions;
using Donation.Application.DTOs.FamilyMember.Request;
using Donation.Application.Features.FamilyMembers.Commands.CreateFamilyMember;
using Donation.Application.Features.FamilyMembers.Commands.DeleteFamilyMember;
using Donation.Application.Features.FamilyMembers.Commands.UpdateFamilyMember;
using Donation.Application.Features.FamilyMembers.Queries.GetAllFamilyMembers;
using Donation.Application.Features.FamilyMembers.Queries.GetFamilyMemberById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FamilyMembersController : BaseController
{
    private readonly ISender _sender;

    public FamilyMembersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? beneficiaryId, CancellationToken cancellationToken)
    {
        try
        {
            var items = await _sender.Send(new GetAllFamilyMembersQuery(beneficiaryId), cancellationToken);
            return CustomResponse(items, "Family members retrieved successfully.");
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
                    request.BeneficiaryId,
                    request.FullName,
                    request.BirthDate,
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFamilyMemberRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _sender.Send(
                new UpdateFamilyMemberCommand(
                    id,
                    request.BeneficiaryId,
                    request.FullName,
                    request.BirthDate,
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
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
}
