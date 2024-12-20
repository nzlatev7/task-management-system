using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Interfaces.Handlers;

namespace TaskManagementSystem.Handlers;

public class MediumPriorityHandler : ITaskPriorityHandler
{
    public async Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext, ILogger logger)
    {
        dbContext.Tasks.Remove(taskEntity);

        var deletedTaskEntity = taskEntity.ToDeletedTaskEntity();
        await dbContext.DeletedTasks.AddAsync(deletedTaskEntity);

        await dbContext.SaveChangesAsync();

        logger.LogInformation(LoggingMessageConstants.TaskMovedSuccessfully, taskEntity.Id);
    }
}
