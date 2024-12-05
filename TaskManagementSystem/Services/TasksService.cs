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

    public async Task<TaskResponseDto> CreateTaskAsync(TaskRequestDto taskDto)
    {
        var categoryExist = await _categoriesService.CategoryExistsAsync(taskDto.CategoryId);

        if (!categoryExist)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);

        var taskEntity = new TaskEntity()
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority ?? Priority.Medium,
            IsCompleted = taskDto.IsCompleted,
            CategoryId = taskDto.CategoryId
        };

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
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId);

        if (taskEntity is null)
            throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        return taskEntity.ToOutDto();
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(int taskId, TaskRequestDto taskDto)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId);

        if (taskEntity is null)
            throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        var categoryExists = await _categoriesService.CategoryExistsAsync(taskDto.CategoryId);

        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);

        taskEntity.Title = taskDto.Title;
        taskEntity.Description = taskDto.Description;
        taskEntity.DueDate = taskDto.DueDate;
        taskEntity.Priority = taskDto.Priority ?? Priority.Medium;
        taskEntity.IsCompleted = taskDto.IsCompleted;
        taskEntity.CategoryId = taskDto.CategoryId;

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
}
