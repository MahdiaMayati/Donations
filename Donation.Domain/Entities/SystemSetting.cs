using Donation.Domain.Common;

namespace Donation.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public Guid? OrganizationId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;

    public Organization? Organization { get; set; }
}
