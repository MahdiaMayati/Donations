using Donation.Application.Common.Exceptions;
using Donation.Application.Common.Pagination;
using Donation.Application.DTOs.Material.Request;
using Donation.Application.Features.Materials.Commands.CreateMaterial;
using Donation.Application.Features.Materials.Commands.CreateMaterialsBulk;
using Donation.Application.Features.Materials.Commands.DeleteMaterial;
using Donation.Application.Features.Materials.Commands.DeleteMaterialsBulk;
using Donation.Application.Features.Materials.Commands.RestoreMaterial;
using Donation.Application.Features.Materials.Commands.RestoreMaterialsBulk;
using Donation.Application.Features.Materials.Commands.UpdateMaterial;
using Donation.Application.Features.Materials.Commands.UpdateMaterialsBulk;
using Donation.Application.Features.Materials.Queries.GetAllMaterials;
using Donation.Application.Features.Materials.Queries.GetDeletedMaterials;
using Donation.Application.Features.Materials.Queries.GetMaterialById;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/v1/materials")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class MaterialsController : BaseController
{
    private readonly ISender _sender;

    public MaterialsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] List<Guid>? ids,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetAllMaterialsQuery(ids, pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Materials retrieved successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpGet("batch")]
    public async Task<IActionResult> GetBatch(
        [FromQuery] List<Guid> ids,
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (ids is null || ids.Count == 0)
            {
                return CustomErrorResponse("At least one id is required.", StatusCodes.Status400BadRequest);
            }

            var result = await _sender.Send(
                new GetAllMaterialsQuery(ids, pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Materials retrieved successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeleted(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _sender.Send(
                new GetDeletedMaterialsQuery(pagination.Page, pagination.Limit, pagination.Search),
                cancellationToken);
            return CustomResponse(result, "Deleted materials retrieved successfully.");
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
            var material = await _sender.Send(new GetMaterialByIdQuery(id), cancellationToken);
            if (material is null)
            {
                return CustomErrorResponse("Material not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(material, "Material retrieved successfully.");
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
        [FromBody] CreateMaterialRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var material = await _sender.Send(new CreateMaterialCommand(request.Name), cancellationToken);
            return CustomResponse(material, "Material created successfully.", StatusCodes.Status201Created);
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

    [HttpPost("batch")]
    public async Task<IActionResult> CreateBatch(
        [FromBody] List<CreateMaterialRequest> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var materials = await _sender.Send(
                new CreateMaterialsBulkCommand(
                    (request ?? new List<CreateMaterialRequest>())
                        .Select(r => new CreateMaterialItem(r.Name))
                        .ToList()),
                cancellationToken);
            return CustomResponse(materials, "Materials created successfully.", StatusCodes.Status201Created);
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
        [FromBody] UpdateMaterialRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var material = await _sender.Send(
                new UpdateMaterialCommand(id, request.Name),
                cancellationToken);
            if (material is null)
            {
                return CustomErrorResponse("Material not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(material, "Material updated successfully.");
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

    [HttpPut("batch")]
    public async Task<IActionResult> UpdateBatch(
        [FromBody] List<UpdateMaterialRequest> request,
        CancellationToken cancellationToken)
    {
        try
        {
            var body = request ?? new List<UpdateMaterialRequest>();
            if (body.Any(r => !r.Id.HasValue || r.Id.Value == Guid.Empty))
            {
                return CustomErrorResponse(
                    "Each item must include a valid Id for bulk update.",
                    StatusCodes.Status400BadRequest);
            }

            var materials = await _sender.Send(
                new UpdateMaterialsBulkCommand(
                    body.Select(r => new UpdateMaterialItem(r.Id!.Value, r.Name)).ToList()),
                cancellationToken);
            return CustomResponse(materials, "Materials updated successfully.");
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
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _sender.Send(new DeleteMaterialCommand(id), cancellationToken);
            if (!deleted)
            {
                return CustomErrorResponse("Material not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(null, "Material deleted successfully.");
        }
        catch (ValidationException ex)
        {
            return CustomErrorResponse(
                "Validation failed.",
                StatusCodes.Status400BadRequest,
                ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }
    }

    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteBatch(
        [FromBody] BulkMaterialIdsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var count = await _sender.Send(
                new DeleteMaterialsBulkCommand(request.Ids ?? new List<Guid>()),
                cancellationToken);
            return CustomResponse(new { deletedCount = count }, "Materials deleted successfully.");
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
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var material = await _sender.Send(new RestoreMaterialCommand(id), cancellationToken);
            if (material is null)
            {
                return CustomErrorResponse("Deleted material not found.", StatusCodes.Status404NotFound);
            }

            return CustomResponse(material, "Material restored successfully.");
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

    [HttpPost("restore/batch")]
    public async Task<IActionResult> RestoreBatch(
        [FromBody] BulkMaterialIdsRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var materials = await _sender.Send(
                new RestoreMaterialsBulkCommand(request.Ids ?? new List<Guid>()),
                cancellationToken);
            return CustomResponse(materials, "Materials restored successfully.");
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
}
