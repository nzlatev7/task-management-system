using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Interfaces;

public interface ITaskDeleteStrategy
{
    bool CanExecute(TaskEntity taskEntity);
    Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext);
}