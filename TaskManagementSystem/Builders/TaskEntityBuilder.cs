using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Builders;

public sealed class TaskEntityBuilder
{
    private readonly TaskEntity _taskEntity;

    private TaskEntityBuilder(
        string title,
        string? description,
        DateTime dueDate,
        int categoryId,
        TaskKind kind)
    {
        _taskEntity = new TaskEntity
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            CategoryId = categoryId,
            Kind = kind,
            Priority = Priority.Medium,
            Status = Status.Pending,
            IsCompleted = false
        };
    }

    public static TaskEntityBuilder Create(
        string title,
        string? description,
        DateTime dueDate,
        int categoryId,
        TaskKind kind)
    {
        return new TaskEntityBuilder(title, description, dueDate, categoryId, kind);
    }

    public TaskEntityBuilder WithPriority(Priority priority)
    {
        _taskEntity.Priority = priority;
        return this;
    }

    public TaskEntityBuilder WithStatus(Status status)
    {
        _taskEntity.Status = status;
        _taskEntity.IsCompleted = status is Status.Completed or Status.Archived;
        return this;
    }

    public TaskEntityBuilder WithStoryPoints(int? storyPoints)
    {
        _taskEntity.StoryPoints = storyPoints;
        return this;
    }

    public TaskEntityBuilder WithDueDate(DateTime dueDate)
    {
        _taskEntity.DueDate = dueDate;
        return this;
    }

    public TaskEntityBuilder WithKind(TaskKind kind)
    {
        _taskEntity.Kind = kind;
        return this;
    }

    public TaskEntityBuilder WithCategory(int categoryId)
    {
        _taskEntity.CategoryId = categoryId;
        return this;
    }

    public TaskEntityBuilder WithCompletionStatus(bool isCompleted)
    {
        _taskEntity.IsCompleted = isCompleted;
        return this;
    }

    public TaskEntity Build()
    {
        return _taskEntity;
    }
}
