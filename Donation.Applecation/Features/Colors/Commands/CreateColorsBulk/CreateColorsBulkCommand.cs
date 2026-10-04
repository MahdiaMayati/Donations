using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Commands.CreateColorsBulk;

public sealed record CreateColorItem(string Name, string Code);

public sealed record CreateColorsBulkCommand(IReadOnlyList<CreateColorItem> Items)
    : IRequest<IReadOnlyList<ColorResponse>>;
