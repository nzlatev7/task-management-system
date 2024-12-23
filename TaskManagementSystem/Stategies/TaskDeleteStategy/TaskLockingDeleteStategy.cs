using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public class TaskLockingDeleteStategy : ITaskDeleteStategy
{
    public async Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        taskEntity.Status = Status.Locked;

        await dbContext.SaveChangesAsync();
    }
}
