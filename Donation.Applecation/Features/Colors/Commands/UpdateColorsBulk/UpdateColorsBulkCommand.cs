using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Commands.UpdateColorsBulk;

public sealed record UpdateColorItem(Guid Id, string Name, string Code);

public sealed record UpdateColorsBulkCommand(IReadOnlyList<UpdateColorItem> Items)
    : IRequest<IReadOnlyList<ColorResponse>>;
