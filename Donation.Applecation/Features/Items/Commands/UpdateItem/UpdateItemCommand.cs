using Donation.Application.DTOs.Item.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.Items.Commands.UpdateItem;

public sealed record UpdateItemCommand(
    Guid Id,
    Guid ItemTypeId,
    Guid? MaterialId,
    Guid? StorageLocationId,
    Guid? SortedByUserId,
    string? Barcode,
    TargetGender TargetGender,
    AgeGroup AgeGroup,
    string Size,
    Season Season,
    ItemCondition Condition,
    ItemSortingStatus SortingStatus,
    ItemAvailabilityStatus AvailabilityStatus,
    string? SorterNotes,
    DateTime? ReceivedAt) : IRequest<ItemResponse?>;
