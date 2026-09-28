using MediatR;

namespace Donation.Application.Features.Areas.Commands.DeleteArea;

public sealed record DeleteAreaCommand(int Id) : IRequest<bool>;
