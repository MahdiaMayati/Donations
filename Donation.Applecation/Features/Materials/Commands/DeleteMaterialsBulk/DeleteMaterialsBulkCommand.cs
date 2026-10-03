using MediatR;

namespace Donation.Application.Features.Materials.Commands.DeleteMaterialsBulk;

public sealed record DeleteMaterialsBulkCommand(IReadOnlyList<Guid> Ids) : IRequest<int>;
