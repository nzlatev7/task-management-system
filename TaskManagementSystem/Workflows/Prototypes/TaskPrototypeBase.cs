using TaskManagementSystem.Builders;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Workflows.Prototypes;

public abstract class TaskPrototypeBase : ITaskPrototype
{
    protected TaskPrototypeBase(TaskEntity prototype)
    {
        Prototype = prototype;
    }

    protected TaskEntity Prototype { get; }

    public TaskEntity Clone(TaskEntity sourceTask, CloneTaskRequestDto? cloneRequest = null)
    {
        if (sourceTask is null)
            throw new ArgumentNullException(nameof(sourceTask));

        cloneRequest ??= new CloneTaskRequestDto();

        var title = string.IsNullOrWhiteSpace(cloneRequest.Title)
            ? sourceTask.Title
            : cloneRequest.Title!;

        var description = cloneRequest.Description ?? sourceTask.Description;
        var dueDate = cloneRequest.DueDate ?? GetDefaultDueDate(sourceTask);
        var categoryId = cloneRequest.CategoryId ?? sourceTask.CategoryId;
        var priority = GetPriority(sourceTask, cloneRequest);
        var status = cloneRequest.Status ?? Prototype.Status;

        var builder = TaskEntityBuilder.Create(
                title,
                description,
                dueDate,
                categoryId,
                sourceTask.Kind)
            .WithPriority(priority)
            .WithStatus(status);

        return Configure(builder, sourceTask, cloneRequest).Build();
    }

    protected virtual DateTime GetDefaultDueDate(TaskEntity sourceTask) => sourceTask.DueDate;

    protected virtual Priority GetPriority(TaskEntity sourceTask, CloneTaskRequestDto cloneRequest)
    {
        return cloneRequest.Priority ?? Prototype.Priority;
    }

    protected abstract TaskEntityBuilder Configure(
        TaskEntityBuilder builder,
        TaskEntity sourceTask,
        CloneTaskRequestDto cloneRequest);
}
