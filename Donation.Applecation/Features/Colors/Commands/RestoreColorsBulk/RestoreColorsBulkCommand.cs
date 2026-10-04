using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Commands.RestoreColorsBulk;

public sealed record RestoreColorsBulkCommand(IReadOnlyList<Guid> Ids)
    : IRequest<IReadOnlyList<ColorResponse>>;
