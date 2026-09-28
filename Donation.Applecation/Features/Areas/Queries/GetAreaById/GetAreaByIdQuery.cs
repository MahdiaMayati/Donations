using Donation.Application.DTOs.Area.Response;
using MediatR;

namespace Donation.Application.Features.Areas.Queries.GetAreaById;

public sealed record GetAreaByIdQuery(int Id) : IRequest<AreaResponse?>;
