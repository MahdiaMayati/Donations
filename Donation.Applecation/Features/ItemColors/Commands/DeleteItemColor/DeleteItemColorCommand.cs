using MediatR;

namespace Donation.Application.Features.ItemColors.Commands.DeleteItemColor;

public sealed record DeleteItemColorCommand(Guid ItemId, Guid ColorId) : IRequest<bool>;
