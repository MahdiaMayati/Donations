using Donation.Application.DTOs.Material.Response;
using MediatR;

namespace Donation.Application.Features.Materials.Commands.RestoreMaterialsBulk;

public sealed record RestoreMaterialsBulkCommand(IReadOnlyList<Guid> Ids)
    : IRequest<IReadOnlyList<MaterialResponse>>;
