using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Services;

public abstract class TasksServiceBase : ITasksService
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryChecker _categoryChecker;
    private readonly ITaskDeleteOrchestrator _taskDeleteOrchestrator;
    private readonly ITaskArtifactsFactory _taskArtifactsFactory;

    protected TasksServiceBase(
        TaskManagementSystemDbContext dbContext,
        ICategoryChecker categoryChecker,
        ITaskDeleteOrchestrator taskDeleteOrchestrator,
        ITaskArtifactsFactory taskArtifactsFactory)
    {
        _dbContext = dbContext;
        _categoryChecker = categoryChecker;
        _taskDeleteOrchestrator = taskDeleteOrchestrator;
        _taskArtifactsFactory = taskArtifactsFactory;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto)
    {
        await ValidatateCategoryAsync(taskDto.CategoryId);

        var workflow = _taskArtifactsFactory.CreateWorkflow(taskDto.Kind);
        workflow.Validate(taskDto);

        var taskEntity = workflow.Build(taskDto);

        await _dbContext.Tasks.AddAsync(taskEntity);
        await _dbContext.SaveChangesAsync();

        return taskEntity.ToOutDto();
    }

    public async Task<TaskResponseDto> CloneTaskAsync(int taskId, CloneTaskRequestDto? cloneDto)
    {
        var sourceTask = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        cloneDto ??= new CloneTaskRequestDto();

        var targetCategoryId = cloneDto.CategoryId ?? sourceTask.CategoryId;
        await ValidatateCategoryAsync(targetCategoryId);

        var prototype = _taskArtifactsFactory.CreatePrototype(sourceTask.Kind);
        var clonedTask = prototype.Clone(sourceTask, cloneDto);

        await _dbContext.Tasks.AddAsync(clonedTask);
        await _dbContext.SaveChangesAsync();

        return clonedTask.ToOutDto();
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
                CategoryId = x.CategoryId,
                Kind = x.Kind
            }).ToListAsync();

        return tasks;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetBacklogAsync(TaskKind kind)
    {
        var backlogOrdering = _taskArtifactsFactory.CreateBacklogOrdering(kind);

        var backlog = await _dbContext.Tasks
            .Where(task =>
                task.Kind == kind &&
                task.Status != Status.Completed &&
                task.Status != Status.Archived)
            .Select(x => new TaskResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DueDate = x.DueDate,
                Priority = x.Priority,
                StoryPoints = x.StoryPoints,
                IsCompleted = x.IsCompleted,
                Status = x.Status,
                CategoryId = x.CategoryId,
                Kind = x.Kind
            })
            .ToListAsync();

        var orderedBacklog = backlogOrdering.Order(backlog).ToList();

        return orderedBacklog;
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
        await ValidatateCategoryAsync(taskDto.CategoryId);

        taskDto.UpdateTaskEntity(taskEntity);

        await _dbContext.SaveChangesAsync();

        return taskEntity.ToOutDto();
    }

    public async Task UnlockTaskAsync(int taskId, UnlockTaskRequestDto unlockDto)
    {
        var updatedRows = await _dbContext.Tasks
            .Where(x => x.Id == taskId && x.Status == Status.Locked)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, unlockDto.Status));

        if (updatedRows is 0)
            throw new NotFoundException(ErrorMessageConstants.LockedTaskWithIdDoesNotExist);
    }

    public async Task<DeleteAction> DeleteTaskAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        ValidateTaskStatusForDeletion(taskEntity);

        var deleteAction = await _taskDeleteOrchestrator.ExecuteDeletionAsync(taskEntity, _dbContext);

        return deleteAction;
    }

    private async Task ValidatateCategoryAsync(int categoryId)
    {
        var categoryExists = await _categoryChecker.CategoryExistsAsync(categoryId);

        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private static void ValidateTaskStatusForDeletion(TaskEntity taskEntity)
    {
        if (taskEntity.Status == Status.Locked)
            throw new ConflictException(ErrorMessageConstants.TaskAlreadyLocked);

        if (taskEntity.Status == Status.Archived)
            throw new ConflictException(ErrorMessageConstants.ArchivedTaskCanNotBeDeleted);
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
