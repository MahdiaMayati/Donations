namespace Donation.Application.DTOs.SystemSetting.Request;

public sealed class CreateSystemSettingRequest
{
    public Guid? OrganizationId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
