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
using TaskManagementSystem.Handlers;

namespace TaskManagementSystem.Services;

public sealed class TasksService : ITasksService
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<TasksService> _logger;

    public TasksService(
        TaskManagementSystemDbContext dbContext,
        ICategoryRepository categoryRepository,
        ILogger<TasksService> logger)
    {
        _dbContext = dbContext; 
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto)
    {
        await ValidatateCategoryExistsAsync(taskDto.CategoryId);

        var taskEntity = taskDto.ToTaskEntityForCreate();

        await _dbContext.Tasks.AddAsync(taskEntity);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(LoggingMessageConstants.TaskCreatedSuccessfully, taskEntity.Id, taskEntity.CategoryId);

        return taskEntity.ToOutDto();
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(GetAllTasksRequestDto sortByInstructions)
    {
        var tasks = await _dbContext.Tasks
            .SortBy(sortByInstructions)
            .Select(x => new TaskResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DueDate = x.DueDate,
                Priority = x.Priority,
                IsCompleted = x.IsCompleted,
                Status = x.Status,
                CategoryId = x.CategoryId
            }).ToListAsync();

        return tasks;
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        return taskEntity.ToOutDto();
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto taskDto)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        ValidateTaskStatusForUpdateAsync(taskEntity, taskDto);
        await ValidatateCategoryExistsAsync(taskDto.CategoryId);

        taskDto.UpdateTaskEntity(taskEntity);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(LoggingMessageConstants.TaskUpdatedSuccessfully, taskEntity.Id);

        return taskEntity.ToOutDto();
    }

    public async Task UnlockTaskAsync(int taskId, UnlockTaskRequestDto unlockDto)
    {
        var updatedRows = await _dbContext.Tasks
            .Where(x => x.Id == taskId && x.Status == Status.Locked)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, unlockDto.Status));

        if (updatedRows is 0)
            throw new NotFoundException(ErrorMessageConstants.LockedTaskWithIdDoesNotExist);

        _logger.LogInformation(LoggingMessageConstants.TaskUnlockedSuccessfully, taskId);
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        if (taskEntity.Status == Status.Locked)
            throw new ConflictException(ErrorMessageConstants.TaskAlreadyLocked);

        if (taskEntity.Status == Status.Archived)
            throw new ConflictException(ErrorMessageConstants.ArchivedTaskCanNotBeDeleted);

        var priorityHandlerFactory = new TaskPriorityHandlerFactory();
        var priorityHandler = priorityHandlerFactory.GetHandler(taskEntity.Priority);

        await priorityHandler.HandleAsync(taskEntity, _dbContext, _logger);
    }

    private async Task ValidatateCategoryExistsAsync(int categoryId)
    {
        var categoryExists = await _categoryRepository.CategoryExistsAsync(categoryId);

        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private void ValidateTaskStatusForUpdateAsync(TaskEntity taskEntity, UpdateTaskRequestDto taskDto)
    {
        var canArchivedBeEdited = taskEntity.Status == Status.Archived;
        if (canArchivedBeEdited)
            throw new ConflictException(ErrorMessageConstants.ArchivedTaskCanNotBeEdited);

        var canArchiveEntity = taskEntity.Status != Status.Completed && taskDto.Status == Status.Archived;
        if (canArchiveEntity)
            throw new ConflictException(ErrorMessageConstants.OnlyCompletedTaskCanBeArchived);

        var canLockedBeEdited = taskEntity.Status == Status.Locked;
        if (canLockedBeEdited)
            throw new ConflictException(ErrorMessageConstants.LockedTaskCanNotBeEdited);
    }
}
