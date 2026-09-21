using Microsoft.AspNetCore.Identity;

namespace Donation.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public int RoleLevel { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // أضيفي هذا السطر لكي يراه الـ AppDbContext وتختفي مشكلة الـ UserRoles تماماً
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}