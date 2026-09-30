using Donation.Application.DTOs.Donor.Response;
using MediatR;

namespace Donation.Application.Features.Donors.Queries.GetDeletedDonors;

public sealed record GetDeletedDonorsQuery : IRequest<IReadOnlyList<DonorResponse>>;
