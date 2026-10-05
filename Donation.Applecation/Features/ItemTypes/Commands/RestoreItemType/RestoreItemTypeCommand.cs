using Donation.Application.DTOs.ItemType.Response;
using MediatR;

namespace Donation.Application.Features.ItemTypes.Commands.RestoreItemType;

public sealed record RestoreItemTypeCommand(Guid Id) : IRequest<ItemTypeResponse?>;
