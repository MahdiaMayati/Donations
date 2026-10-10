using Donation.Application.DTOs.Item.Response;
using MediatR;

namespace Donation.Application.Features.Items.Queries.GetItemById;

public sealed record GetItemByIdQuery(Guid Id) : IRequest<ItemResponse?>;
