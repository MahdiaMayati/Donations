using Donation.Application.DTOs.Color.Response;
using MediatR;

namespace Donation.Application.Features.Colors.Commands.CreateColor;

public sealed record CreateColorCommand(string Name, string Code) : IRequest<ColorResponse>;
