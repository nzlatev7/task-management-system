using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows;

public sealed class IncidentWorkflow : ITaskWorkflow
{
    public void Validate(CreateTaskRequestDto dto) { }

    public TaskEntity Build(CreateTaskRequestDto dto)
    {
        var dueDate = dto.DueDate != default ? dto.DueDate : DateTime.UtcNow.AddHours(4);

        return new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dueDate,
            Priority = Priority.High,
            IsCompleted = false,
            Status = Status.InProgress,
            CategoryId = dto.CategoryId,
            Kind = TaskKind.Incident
        };
    }
}
