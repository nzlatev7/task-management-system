using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Database;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Extensions;

namespace TaskManagementSystem.Services;

public sealed class TasksService : ITasksService
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoriesService _categoriesService;

    public TasksService(TaskManagementSystemDbContext dbContext, ICategoriesService categoriesService)
    {
        _dbContext = dbContext;
        _categoriesService = categoriesService;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto)
    {
        await ValidatateCategoryExists(taskDto.CategoryId);

        var taskEntity = new TaskEntity()
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            IsCompleted = false,
            Status = Status.Pending,
            CategoryId = taskDto.CategoryId
        };

        if (taskDto.Priority.HasValue)
            taskEntity.Priority = taskDto.Priority.Value;

        await _dbContext.Tasks.AddAsync(taskEntity);
        await _dbContext.SaveChangesAsync();

        return taskEntity.ToOutDto();
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(bool sortByPriorityAscending)
    {
        var tasks = await _dbContext.Tasks
            .SortByPriority(sortByPriorityAscending)
            .ToOutDtos();

        return tasks;
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(int taskId)
    {
        var taskEntity = await GetTaskEntityAsync(taskId);

        return taskEntity.ToOutDto();
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto taskDto)
    {
        var taskEntity = await GetTaskEntityAsync(taskId);

        await ValidateTaskDataForUpdate(taskEntity, taskDto);

        taskEntity.Title = taskDto.Title;
        taskEntity.Description = taskDto.Description;
        taskEntity.DueDate = taskDto.DueDate;
        taskEntity.IsCompleted = IsCompleted(taskDto.Status);
        taskEntity.Status = taskDto.Status;
        taskEntity.CategoryId = taskDto.CategoryId;

        if (taskDto.Priority.HasValue)
            taskEntity.Priority = taskDto.Priority.Value;

        await _dbContext.SaveChangesAsync();

        return taskEntity.ToOutDto();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var deletedRows = await _dbContext.Tasks
            .Where(x => x.Id == taskId)
            .ExecuteDeleteAsync();

        if (deletedRows is 0)
            throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);
    }

    private async Task<TaskEntity> GetTaskEntityAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId) 
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        return taskEntity;
    }

    private async Task ValidatateCategoryExists(int categoryId)
    {
        var categoryExists = await _categoriesService.CategoryExistsAsync(categoryId);

        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private bool IsCompleted(Status taskStatus)
    {
        return taskStatus == Status.Completed || taskStatus == Status.Archived
            ? true
            : false;
    }

    private async Task ValidateTaskDataForUpdate(TaskEntity taskEntity, UpdateTaskRequestDto taskDto)
    {
        var canArchivedBeEdited = taskEntity.Status == Status.Archived;
        if (canArchivedBeEdited)
            throw new BadHttpRequestException(ErrorMessageConstants.ArchivedTaskCanNotBeEdited);

        var canArchiveEntity = taskEntity.Status != Status.Completed && taskDto.Status == Status.Archived;
        if (canArchiveEntity)
            throw new BadHttpRequestException(ErrorMessageConstants.OnlyCompletedTaskCanBeArchived);

        await ValidatateCategoryExists(taskDto.CategoryId);
    }
}
