using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public sealed class TaskMovingDeleteStategy : ITaskDeleteStategy
{
    private const DeleteAction deleteAction = DeleteAction.Moved;

    public async Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        dbContext.Tasks.Remove(taskEntity);

        var deletedTaskEntity = taskEntity.ToDeletedTaskEntity();
        await dbContext.DeletedTasks.AddAsync(deletedTaskEntity);

        await dbContext.SaveChangesAsync();

        return deleteAction;
    }
}
