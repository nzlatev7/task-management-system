using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public class TaskRemovingDeleteStategy : ITaskDeleteStategy
{
    public async Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        dbContext.Tasks.Remove(taskEntity);

        await dbContext.SaveChangesAsync();
    }
}