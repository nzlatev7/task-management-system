﻿using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.TaskDeleteStategy;

public class TaskLockingDeleteStrategy : ITaskDeleteStrategy
{
    private const DeleteAction deleteAction = DeleteAction.Locked;

    public bool CanExecute(TaskEntity taskEntity)
    {
        return taskEntity.Priority == Priority.High;
    }

    public async Task<DeleteAction> DeleteAsync(TaskEntity taskEntity, TaskManagementSystemDbContext dbContext)
    {
        taskEntity.Status = Status.Locked;

        await dbContext.SaveChangesAsync();

        return deleteAction;
    }
}
