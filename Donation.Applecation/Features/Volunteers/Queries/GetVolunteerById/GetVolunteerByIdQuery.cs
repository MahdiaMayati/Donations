using Donation.Application.DTOs.Volunteer.Response;
using MediatR;

namespace Donation.Application.Features.Volunteers.Queries.GetVolunteerById;

public sealed record GetVolunteerByIdQuery(int Id) : IRequest<VolunteerResponse?>;
