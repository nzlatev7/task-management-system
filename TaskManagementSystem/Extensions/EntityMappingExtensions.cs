using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Extensions;

public static class EntityMappingExtensions
{
    public static CategoryEntity ToCategoryEntityForCreate(this CategoryRequestDto categoryDto)
    {
        return new CategoryEntity()
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };
    }

    public static void UpdateCategoryEntity(this CategoryRequestDto categoryDto, CategoryEntity category)
    {
        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;
    }

    public static void UpdateTaskEntity(this UpdateTaskRequestDto taskDto, TaskEntity currentTask)
    {
        currentTask.Title = taskDto.Title;
        currentTask.Description = taskDto.Description;
        currentTask.DueDate = taskDto.DueDate;
        currentTask.IsCompleted = IsCompleted(taskDto.Status);
        currentTask.Status = taskDto.Status;
        currentTask.CategoryId = taskDto.CategoryId;

        if (taskDto.Priority.HasValue)
            currentTask.Priority = taskDto.Priority.Value;
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
