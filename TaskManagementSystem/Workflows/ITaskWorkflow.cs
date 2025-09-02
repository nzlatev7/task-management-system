using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Workflows;

public interface ITaskWorkflow
{
    void Validate(CreateTaskRequestDto dto);
    TaskEntity Build(CreateTaskRequestDto dto);
}
