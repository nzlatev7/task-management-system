using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public sealed class TaskDeleteContext : ITaskDeleteContext
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly Dictionary<DeleteAction, ITaskDeleteStategy> _strategies;

    public TaskDeleteContext(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;

        _strategies = new Dictionary<DeleteAction, ITaskDeleteStategy>
        {
            { DeleteAction.Removed, new TaskRemovingDeleteStategy() },
            { DeleteAction.Moved, new TaskMovingDeleteStategy() },
            { DeleteAction.Locked, new TaskLockingDeleteStategy() }
        };
    }

    public async Task HandleAsync(TaskEntity task, DeleteAction instruction)
    {
        await _strategies[instruction].HandleAsync(task, _dbContext);
    }
}