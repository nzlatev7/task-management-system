using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Tasks.CreateTask.Extensions;

public static class CreateTaskMappingExtensions
{
    public static TaskEntity ToTaskEntityForCreate(this CreateTaskCommand request)
    {
        var taskEntity = new TaskEntity()
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Status = Status.Pending,
            CategoryId = request.CategoryId
        };

        if (request.Priority.HasValue)
            taskEntity.Priority = request.Priority.Value;

        return taskEntity;
    }
}
