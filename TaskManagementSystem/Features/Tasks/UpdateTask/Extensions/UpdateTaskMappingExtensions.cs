using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Features.Tasks.UpdateTask.Extensions;

public static class UpdateTaskMappingExtensions
{
    public static void UpdateTaskEntity(this UpdateTaskCommand request, TaskEntity currentTask)
    {
        currentTask.Title = request.Title;
        currentTask.Description = request.Description;
        currentTask.DueDate = request.DueDate;
        currentTask.Status = request.Status;
        currentTask.CategoryId = request.CategoryId;

        if (request.Priority.HasValue)
            currentTask.Priority = request.Priority.Value;
    }
}