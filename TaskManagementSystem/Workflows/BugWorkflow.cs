using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows;

public sealed class BugWorkflow : ITaskWorkflow
{
    public void Validate(CreateTaskRequestDto dto)
    {
        if (!dto.Severity.HasValue)
            throw new ArgumentException("Bug requires Severity.", nameof(dto.Severity));
        if (dto.Severity is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(dto.Severity), "Severity must be between 1 and 5.");
    }

    public TaskEntity Build(CreateTaskRequestDto dto)
    {
        var severity = dto.Severity!.Value;
        var priority = severity >= 4 ? Priority.High : Priority.Low;

        return new TaskEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            Priority = priority,
            IsCompleted = false,
            Status = Status.Pending,
            CategoryId = dto.CategoryId,
            Kind = TaskKind.Bug
        };
    }
}