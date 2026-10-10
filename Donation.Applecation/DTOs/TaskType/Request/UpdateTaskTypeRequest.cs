namespace Donation.Application.DTOs.TaskType.Request;

public sealed class UpdateTaskTypeRequest
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
