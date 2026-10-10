using Donation.Application.DTOs.ItemStatusHistory.Response;
using MediatR;

namespace Donation.Application.Features.ItemStatusHistories.Queries.GetItemStatusHistoryById;

public sealed record GetItemStatusHistoryByIdQuery(Guid Id) : IRequest<ItemStatusHistoryResponse?>;
