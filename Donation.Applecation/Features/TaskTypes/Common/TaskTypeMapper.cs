using Donation.Application.DTOs.TaskType.Response;
using Donation.Domain.Entities;

namespace Donation.Application.Features.TaskTypes.Common;

internal static class TaskTypeMapper
{
    public static TaskTypeResponse Map(TaskType taskType) => new()
    {
        Id = taskType.Id,
        Code = taskType.Code,
        Name = taskType.Name
    };
}
