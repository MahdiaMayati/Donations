namespace Donation.Domain.Enums;

/// <summary>
/// Donation request lifecycle (operational workflow).
/// </summary>
public enum DonationRequestStatus
{
    Submitted = 1,
    Scheduled = 2,
    AssignedToVolunteer = 3,
    Received = 4,
    Sorting = 5,
    Sorted = 6
}
