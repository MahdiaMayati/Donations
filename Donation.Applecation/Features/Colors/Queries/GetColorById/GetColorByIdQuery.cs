using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Queries.GetColorById;

public sealed record GetColorByIdQuery(Guid Id) : IRequest<ColorResponse?>;
