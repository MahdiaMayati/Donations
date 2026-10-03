using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Commands.RestoreColor;

public sealed record RestoreColorCommand(Guid Id) : IRequest<ColorResponse?>;
