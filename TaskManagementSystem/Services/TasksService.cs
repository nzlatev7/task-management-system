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
    private readonly ICategoryRepository _categoryRepository;

    public TasksService(TaskManagementSystemDbContext dbContext, ICategoryRepository categoryRepository)
    {
        _dbContext = dbContext;
        _categoryRepository = categoryRepository;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto)
    {
        await ValidatateCategoryExistsAsync(taskDto.CategoryId);

        var taskEntity = taskDto.ToTaskEntityForCreate();

        await _dbContext.Tasks.AddAsync(taskEntity);
        await _dbContext.SaveChangesAsync();

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

        await ValidateTaskDataForUpdateAsync(taskEntity, taskDto);

        taskDto.UpdateTaskEntity(taskEntity);

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

    private async Task ValidatateCategoryExistsAsync(int categoryId)
    {
        var categoryExists = await _categoryRepository.CategoryExistsAsync(categoryId);

        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private async Task ValidateTaskDataForUpdateAsync(TaskEntity taskEntity, UpdateTaskRequestDto taskDto)
    {
        var canArchivedBeEdited = taskEntity.Status == Status.Archived;
        if (canArchivedBeEdited)
            throw new BadHttpRequestException(ErrorMessageConstants.ArchivedTaskCanNotBeEdited);

        var canArchiveEntity = taskEntity.Status != Status.Completed && taskDto.Status == Status.Archived;
        if (canArchiveEntity)
            throw new BadHttpRequestException(ErrorMessageConstants.OnlyCompletedTaskCanBeArchived);

        await ValidatateCategoryExistsAsync(taskDto.CategoryId);
    }
}
