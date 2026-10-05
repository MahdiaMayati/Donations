using Donation.Application.DTOs.ItemType.Response;
using MediatR;

namespace Donation.Application.Features.ItemTypes.Commands.CreateItemType;

public sealed record CreateItemTypeCommand(
    Guid CategoryId,
    string Name,
    decimal OutfitUnits) : IRequest<ItemTypeResponse>;
