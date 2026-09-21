using Microsoft.AspNetCore.Identity;

namespace Donation.Domain.Entities;

public class UserRole : IdentityUserRole<Guid>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}