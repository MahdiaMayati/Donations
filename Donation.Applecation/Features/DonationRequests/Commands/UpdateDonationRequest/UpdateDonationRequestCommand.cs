using Donation.Application.DTOs.DonationRequest.Request;
using Donation.Application.DTOs.DonationRequest.Response;
using Donation.Domain.Enums;
using MediatR;

namespace Donation.Application.Features.DonationRequests.Commands.UpdateDonationRequest;

public sealed record UpdateDonationRequestCommand(
    Guid Id,
    Guid? PickupAddressId,
    DeliveryMethod DeliveryMethod,
    string Description,
    int EstimatedItemCount,
    IReadOnlyList<string> RequestPhotoUrls,
    IReadOnlyList<DonationRequestItemRequest> Items) : IRequest<DonationRequestResponse?>;
