using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public class TaskLockingDeleteStategy : ITaskDeleteStategy
{
    private const DeleteAction deleteAction = DeleteAction.Locked;

    public async Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        taskEntity.Status = Status.Locked;

        await dbContext.SaveChangesAsync();

        return deleteAction;
    }
}
