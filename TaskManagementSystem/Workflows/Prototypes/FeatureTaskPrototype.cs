using TaskManagementSystem.Builders;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows.Prototypes;

public sealed class FeatureTaskPrototype : TaskPrototypeBase
{
    public FeatureTaskPrototype()
        : base(new TaskEntity
        {
            Kind = TaskKind.Feature,
            Priority = Priority.Medium,
            Status = Status.Pending,
            IsCompleted = false
        })
    {
    }

    protected override DateTime GetDefaultDueDate(TaskEntity sourceTask)
    {
        return DateTime.UtcNow.AddDays(14);
    }

    protected override Priority GetPriority(TaskEntity sourceTask, CloneTaskRequestDto cloneRequest)
    {
        return cloneRequest.Priority ?? sourceTask.Priority;
    }

    protected override TaskEntityBuilder Configure(
        TaskEntityBuilder builder,
        TaskEntity sourceTask,
        CloneTaskRequestDto cloneRequest)
    {
        var storyPoints = cloneRequest.StoryPoints ?? sourceTask.StoryPoints;

        if (!storyPoints.HasValue)
            throw new InvalidOperationException("Feature requires StoryPoints to clone.");

        return builder.WithStoryPoints(storyPoints.Value);
    }
}
