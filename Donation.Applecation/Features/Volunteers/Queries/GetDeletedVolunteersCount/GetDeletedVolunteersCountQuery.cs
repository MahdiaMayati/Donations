using MediatR;

namespace Donation.Application.Features.Volunteers.Queries.GetDeletedVolunteersCount;

public sealed record GetDeletedVolunteersCountQuery : IRequest<int>;
