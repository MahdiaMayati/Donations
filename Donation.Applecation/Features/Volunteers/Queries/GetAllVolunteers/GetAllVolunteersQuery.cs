using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Queries.GetAllVolunteers;

public sealed record GetAllVolunteersQuery() : IRequest<IReadOnlyList<VolunteerResponse>>;
