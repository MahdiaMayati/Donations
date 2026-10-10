using Donation.Application.DTOs.ItemType.Response;
using MediatR;

namespace Donation.Application.Features.ItemTypes.Queries.GetItemTypeById;

public sealed record GetItemTypeByIdQuery(Guid Id) : IRequest<ItemTypeResponse?>;
