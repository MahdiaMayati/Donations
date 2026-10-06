using Donation.Domain.Enums;

namespace Donation.Application.Features.DonationRequests.Common;

internal static class DonationRequestStatusTransitions
{
    public static IReadOnlyList<DonationRequestStatus> GetAllowedNext(
        DonationRequestStatus current,
        DeliveryMethod deliveryMethod)
    {
        return (current, deliveryMethod) switch
        {
            (DonationRequestStatus.Submitted, DeliveryMethod.VolunteerPickup) =>
                new[] { DonationRequestStatus.Scheduled },
            (DonationRequestStatus.Submitted, DeliveryMethod.SelfDropOff) =>
                new[] { DonationRequestStatus.Received },
            (DonationRequestStatus.Scheduled, DeliveryMethod.VolunteerPickup) =>
                new[] { DonationRequestStatus.AssignedToVolunteer },
            (DonationRequestStatus.AssignedToVolunteer, DeliveryMethod.VolunteerPickup) =>
                new[] { DonationRequestStatus.Received },
            (DonationRequestStatus.Received, _) => new[] { DonationRequestStatus.Sorting },
            (DonationRequestStatus.Sorting, _) => new[] { DonationRequestStatus.Sorted },
            _ => Array.Empty<DonationRequestStatus>()
        };
    }

    public static bool CanTransition(
        DonationRequestStatus current,
        DonationRequestStatus next,
        DeliveryMethod deliveryMethod)
        => GetAllowedNext(current, deliveryMethod).Contains(next);
}
