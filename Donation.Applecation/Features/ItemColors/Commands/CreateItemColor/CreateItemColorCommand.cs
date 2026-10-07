using Donation.Application.DTOs.ItemColor.Response;
using MediatR;

namespace Donation.Application.Features.ItemColors.Commands.CreateItemColor;

public sealed record CreateItemColorCommand(Guid ItemId, Guid ColorId) : IRequest<ItemColorResponse>;
