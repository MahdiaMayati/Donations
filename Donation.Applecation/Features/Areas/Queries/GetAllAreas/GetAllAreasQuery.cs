using Donation.Application.DTOs.Area.Response;
using MediatR;

namespace Donation.Application.Features.Areas.Queries.GetAllAreas;

public sealed record GetAllAreasQuery(Guid? CityId) : IRequest<IReadOnlyList<AreaResponse>>;
