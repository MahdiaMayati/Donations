using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Commands.UpdateMaterialsBulk;

public sealed record UpdateMaterialItem(Guid Id, string Name);

public sealed record UpdateMaterialsBulkCommand(IReadOnlyList<UpdateMaterialItem> Items)
    : IRequest<IReadOnlyList<MaterialResponse>>;
