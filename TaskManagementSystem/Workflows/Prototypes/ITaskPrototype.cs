using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;

namespace TaskManagementSystem.Workflows.Prototypes;

public interface ITaskPrototype
{
    TaskEntity Clone(TaskEntity sourceTask, CloneTaskRequestDto? cloneRequest = null);
}
