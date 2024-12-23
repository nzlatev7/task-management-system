using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Interfaces;

public interface ITaskDeleteStategy
{
    Task HandleAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext);
}