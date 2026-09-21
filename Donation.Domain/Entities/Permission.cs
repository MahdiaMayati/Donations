namespace Donation.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid(); // يجب أن يكون Guid
    public string Name { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}