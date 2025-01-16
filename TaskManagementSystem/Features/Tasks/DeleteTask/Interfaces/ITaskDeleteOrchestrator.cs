using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;

public interface ITaskDeleteOrchestrator
{
    public Task<DeleteAction> ExecuteDeletionAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext);
}