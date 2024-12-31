using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Interfaces;

public interface ITaskDeleteOrchestrator
{
    public Task<DeleteAction> ExecuteDeletionAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext);
}