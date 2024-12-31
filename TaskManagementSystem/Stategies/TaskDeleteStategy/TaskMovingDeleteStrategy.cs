using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public sealed class TaskMovingDeleteStrategy : ITaskDeleteStrategy
{
    private const DeleteAction deleteAction = DeleteAction.Moved;

    public bool CanExecute(TaskEntity taskEntity)
    {
        return taskEntity.Priority == Priority.Medium;
    }

    public async Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        dbContext.Tasks.Remove(taskEntity);

        var deletedTaskEntity = taskEntity.ToDeletedTaskEntity();
        await dbContext.DeletedTasks.AddAsync(deletedTaskEntity);

        await dbContext.SaveChangesAsync();

        return deleteAction;
    }
}
