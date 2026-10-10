using Donation.Application.DTOs.Item.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.Items.Commands.UpdateItemSortingStatus;

public sealed record UpdateItemSortingStatusCommand(
    Guid Id,
    ItemSortingStatus SortingStatus) : IRequest<ItemResponse?>;
