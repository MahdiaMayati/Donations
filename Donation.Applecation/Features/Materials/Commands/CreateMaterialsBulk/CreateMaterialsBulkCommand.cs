using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Commands.CreateMaterialsBulk;

public sealed record CreateMaterialItem(string Name);

public sealed record CreateMaterialsBulkCommand(IReadOnlyList<CreateMaterialItem> Items)
    : IRequest<IReadOnlyList<MaterialResponse>>;
