using TaskManagementSystem.Builders;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows.Prototypes;

public sealed class BugTaskPrototype : TaskPrototypeBase
{
    public BugTaskPrototype()
        : base(new TaskEntity
        {
            Kind = TaskKind.Bug,
            Priority = Priority.Low,
            Status = Status.Pending,
            IsCompleted = false
        })
    {
    }

    protected override DateTime GetDefaultDueDate(TaskEntity sourceTask)
    {
        return DateTime.UtcNow.AddDays(2);
    }

    protected override Priority GetPriority(TaskEntity sourceTask, CloneTaskRequestDto cloneRequest)
    {
        if (cloneRequest.Priority.HasValue)
            return cloneRequest.Priority.Value;

        if (cloneRequest.Severity.HasValue)
            return cloneRequest.Severity.Value >= 4 ? Priority.High : Priority.Low;

        return sourceTask.Priority;
    }

    protected override TaskEntityBuilder Configure(
        TaskEntityBuilder builder,
        TaskEntity sourceTask,
        CloneTaskRequestDto cloneRequest)
    {
        return builder.WithStoryPoints(null);
    }
}
