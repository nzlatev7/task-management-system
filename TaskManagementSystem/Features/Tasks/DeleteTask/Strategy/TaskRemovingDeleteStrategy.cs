using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;

namespace TaskManagementSystem.Features.Tasks.DeleteTask.Strategy;

public class TaskRemovingDeleteStrategy : ITaskDeleteStrategy
{
    private const DeleteAction deleteAction = DeleteAction.Removed;

    public bool CanExecute(TaskEntity taskEntity)
    {
        return taskEntity.Priority == Priority.Low;
    }

    public async Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        dbContext.Tasks.Remove(taskEntity);

        await dbContext.SaveChangesAsync();

        return deleteAction;
    }
}