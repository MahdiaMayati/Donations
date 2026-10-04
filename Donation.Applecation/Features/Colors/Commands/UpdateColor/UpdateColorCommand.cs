using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Commands.UpdateColor;

public sealed record UpdateColorCommand(Guid Id, string Name, string Code) : IRequest<ColorResponse?>;
