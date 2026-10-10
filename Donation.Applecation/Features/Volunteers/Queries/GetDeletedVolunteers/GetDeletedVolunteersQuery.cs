using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Queries.GetDeletedVolunteers;

public sealed record GetDeletedVolunteersQuery : IRequest<IReadOnlyList<VolunteerResponse>>;
