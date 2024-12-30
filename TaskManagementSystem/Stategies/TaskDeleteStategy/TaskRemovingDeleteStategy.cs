using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public class TaskRemovingDeleteStategy : ITaskDeleteStategy
{
    private const DeleteAction deleteAction = DeleteAction.Removed;

    public async Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        dbContext.Tasks.Remove(taskEntity);

        await dbContext.SaveChangesAsync();

        return deleteAction;
    }
}