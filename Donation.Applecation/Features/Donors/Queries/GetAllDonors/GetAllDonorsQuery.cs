using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Queries.GetAllDonors;

public sealed record GetAllDonorsQuery() : IRequest<IReadOnlyList<DonorResponse>>;
