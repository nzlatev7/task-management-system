using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public sealed class TaskMovingDeleteStategy : ITaskDeleteStategy
{
    public async Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        dbContext.Tasks.Remove(taskEntity);

        var deletedTaskEntity = taskEntity.ToDeletedTaskEntity();
        await dbContext.DeletedTasks.AddAsync(deletedTaskEntity);

        await dbContext.SaveChangesAsync();
    }
}
