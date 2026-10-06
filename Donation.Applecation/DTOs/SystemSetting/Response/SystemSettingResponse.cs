namespace Donation.Application.DTOs.SystemSetting.Response;

public sealed class SystemSettingResponse
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
