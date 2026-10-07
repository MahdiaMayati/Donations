using Donation.Application.DTOs.Item.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.Items.Commands.CreateItem;

public sealed record CreateItemCommand(
    Guid? OrganizationId,
    Guid DonationRequestId,
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
    ItemAvailabilityStatus? AvailabilityStatus,
    string? SorterNotes,
    DateTime? ReceivedAt) : IRequest<ItemResponse>;
