using MediatR;

namespace Donation.Application.Features.Colors.Commands.DeleteColor;

public sealed record DeleteColorCommand(Guid Id) : IRequest<bool>;
