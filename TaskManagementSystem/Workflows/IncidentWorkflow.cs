using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows;

public sealed class IncidentWorkflow : ITaskWorkflow
{
    public void Validate(CreateTaskRequestDto dto) { }

    public TaskEntity Build(CreateTaskRequestDto dto)
    {
        return new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = Priority.High,
            IsCompleted = false,
            Status = Status.InProgress,
            CategoryId = dto.CategoryId,
            Kind = TaskKind.Incident
        };
    }
}