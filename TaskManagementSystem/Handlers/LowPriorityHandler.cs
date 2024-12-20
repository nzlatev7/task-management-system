using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Interfaces.Handlers;

namespace TaskManagementSystem.Handlers;

public class LowPriorityHandler : ITaskPriorityHandler
{
    public async Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext, ILogger logger)
    {
        dbContext.Tasks.Remove(taskEntity);
        await dbContext.SaveChangesAsync();

        logger.LogInformation(LoggingMessageConstants.TaskRemovedSuccessfully, taskEntity.Id);
    }
}
