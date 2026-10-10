using MediatR;

namespace Donation.Application.Features.ItemPhotos.Commands.DeleteItemPhoto;

public sealed record DeleteItemPhotoCommand(Guid Id) : IRequest<bool>;
