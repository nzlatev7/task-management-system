using TaskManagementSystem.Builders;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows.Prototypes;

public sealed class IncidentTaskPrototype : TaskPrototypeBase
{
    public IncidentTaskPrototype()
        : base(new TaskEntity
        {
            Kind = TaskKind.Incident,
            Priority = Priority.High,
            Status = Status.InProgress,
            IsCompleted = false
        })
    {
    }

    protected override DateTime GetDefaultDueDate(TaskEntity sourceTask)
    {
        return DateTime.UtcNow.AddHours(4);
    }

    protected override Priority GetPriority(TaskEntity sourceTask, CloneTaskRequestDto cloneRequest)
    {
        return cloneRequest.Priority ?? Priority.High;
    }

    protected override TaskEntityBuilder Configure(
        TaskEntityBuilder builder,
        TaskEntity sourceTask,
        CloneTaskRequestDto cloneRequest)
    {
        return builder.WithStoryPoints(null);
    }
}
