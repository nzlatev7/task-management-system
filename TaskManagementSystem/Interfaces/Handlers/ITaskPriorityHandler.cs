using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Interfaces.Handlers;

public interface ITaskPriorityHandler
{
    Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext, ILogger logger);
}
