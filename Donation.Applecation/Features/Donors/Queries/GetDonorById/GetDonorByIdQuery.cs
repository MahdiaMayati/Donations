using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Queries.GetDonorById;

public sealed record GetDonorByIdQuery(int Id) : IRequest<DonorResponse?>;
