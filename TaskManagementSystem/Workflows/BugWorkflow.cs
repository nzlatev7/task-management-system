using TaskManagementSystem.Builders;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows;

public sealed class BugWorkflow : ITaskWorkflow
{
    public void Validate(CreateTaskRequestDto dto)
    {
        if (!dto.Severity.HasValue)
            throw new ArgumentException("Bug requires Severity.");
    }

    public TaskEntity Build(CreateTaskRequestDto dto)
    {
        var priority = dto.Severity >= 4 ? Priority.High : Priority.Low;
        var dueDate = dto.DueDate != default ? dto.DueDate : DateTime.UtcNow.AddDays(2);

        return TaskEntityBuilder.Create(
                dto.Title,
                dto.Description,
                dueDate,
                dto.CategoryId,
                TaskKind.Bug)
            .WithPriority(priority)
            .Build();
    }
}
