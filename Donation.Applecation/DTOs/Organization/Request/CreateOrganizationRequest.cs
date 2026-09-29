namespace Donation.Application.DTOs.Organization.Request;

public class CreateOrganizationRequest
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
