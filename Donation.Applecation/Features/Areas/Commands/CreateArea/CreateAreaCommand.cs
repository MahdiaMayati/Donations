using Donation.Application.DTOs.Area.Response;
using MediatR;

namespace Donation.Application.Features.Areas.Commands.CreateArea;

public sealed record CreateAreaCommand(Guid CityId, string Name) : IRequest<AreaResponse>;
