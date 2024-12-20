using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces.Handlers;

namespace TaskManagementSystem.Handlers;

public sealed class HighPriorityHandler : ITaskPriorityHandler
{
    public async Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext, ILogger logger)
    {
        taskEntity.Status = Status.Locked;

        await dbContext.SaveChangesAsync();

        logger.LogInformation(LoggingMessageConstants.TaskLockedSuccessfully, taskEntity.Id);
    }
}
