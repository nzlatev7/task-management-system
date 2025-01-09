using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Tasks.UpdateTask.Extensions;

public static class UpdateTaskMappingExtensions
{
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

    private static bool IsCompleted(Status taskStatus)
    {
        return taskStatus == Status.Completed || taskStatus == Status.Archived;
    }
}
