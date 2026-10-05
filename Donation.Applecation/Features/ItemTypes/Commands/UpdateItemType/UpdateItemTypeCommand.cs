using Donation.Application.DTOs.ItemType.Response;
using MediatR;

namespace Donation.Application.Features.ItemTypes.Commands.UpdateItemType;

public sealed record UpdateItemTypeCommand(
    Guid Id,
    Guid CategoryId,
    string Name,
    decimal OutfitUnits) : IRequest<ItemTypeResponse?>;
