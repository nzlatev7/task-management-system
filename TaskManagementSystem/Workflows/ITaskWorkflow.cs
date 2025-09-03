using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;

namespace TaskManagementSystem.Workflows;

public interface ITaskWorkflow
{
    void Validate(CreateTaskRequestDto dto);
    TaskEntity Build(CreateTaskRequestDto dto);
}