using Donation.Application.DTOs.Area.Response;
using MediatR;

namespace Donation.Application.Features.Areas.Commands.UpdateArea;

public sealed record UpdateAreaCommand(Guid Id, Guid CityId, string Name) : IRequest<AreaResponse?>;
