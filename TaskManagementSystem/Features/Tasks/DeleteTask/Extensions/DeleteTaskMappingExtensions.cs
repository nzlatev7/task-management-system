using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Features.Tasks.DeleteTask.Extensions;

public static class DeleteTaskMappingExtensions
{
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
}
