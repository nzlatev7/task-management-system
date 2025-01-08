using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Tasks;

namespace TaskManagementSystem.Extensions;

public static class EntityMappingExtensions
{
    public static CategoryEntity ToCategoryEntityForCreate(this CreateCategoryCommand request)
    {
        return new CategoryEntity()
        {
            Name = request.Name,
            Description = request.Description
        };
    }

    public static void UpdateCategoryEntity(this UpdateCategoryCommand request, CategoryEntity category)
    {
        category.Name = request.Name;
        category.Description = request.Description;
    }

    public static TaskEntity ToTaskEntityForCreate(this CreateTaskCommand request)
    {
        var taskEntity = new TaskEntity()
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            IsCompleted = false,
            Status = Status.Pending,
            CategoryId = request.CategoryId
        };

        if (request.Priority.HasValue)
            taskEntity.Priority = request.Priority.Value;

        return taskEntity;
    }

    public static void UpdateTaskEntity(this UpdateTaskCommand request, TaskEntity currentTask)
    {
        currentTask.Title = request.Title;
        currentTask.Description = request.Description;
        currentTask.DueDate = request.DueDate;
        currentTask.IsCompleted = IsCompleted(request.Status);
        currentTask.Status = request.Status;
        currentTask.CategoryId = request.CategoryId;

        if (request.Priority.HasValue)
            currentTask.Priority = request.Priority.Value;
    }

    public static DeletedTaskEntity ToDeletedTaskEntity(this TaskEntity task)
    {
        var taskEntity = new DeletedTaskEntity()
        {
            TaskId = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            CategoryId = task.CategoryId
        };

        return taskEntity;
    }

    private static bool IsCompleted(Status taskStatus)
    {
        return taskStatus == Status.Completed || taskStatus == Status.Archived;
    }
}
