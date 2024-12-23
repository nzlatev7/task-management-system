using TaskManagementSystem.Enums;
using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Interfaces;

public interface ITaskDeleteContext
{
    public Task HandleAsync(TaskEntity task, DeleteAction instruction);
}