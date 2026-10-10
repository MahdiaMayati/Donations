using MediatR;

namespace Donation.Application.Features.Colors.Commands.DeleteColorsBulk;

public sealed record DeleteColorsBulkCommand(IReadOnlyList<Guid> Ids) : IRequest<int>;
