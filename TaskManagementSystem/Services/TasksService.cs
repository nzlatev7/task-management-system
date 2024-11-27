using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Models;

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
            throw new ArgumentException(ValidationMessages.CategoryDoesNotExist);

        var taskEntity = new TaskEntity()
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            IsCompleted = taskDto.IsCompleted,
            CategoryId = taskDto.CategoryId
        };

        await _dbContext.Tasks.AddAsync(taskEntity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return taskEntity.ToOutDto();
    }
    public async Task<List<TaskResponseDto>> GetAllTasksAsync()
    {
        var result = new List<TaskResponseDto>();

        var tasks = await _dbContext.Tasks.ToListAsync().ConfigureAwait(false);

        foreach (var taskEntity in tasks)
        {
            result.Add(taskEntity.ToOutDto());
        }

        return result;
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId).ConfigureAwait(false);

        var result = taskEntity?.ToOutDto();

        return result;
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(int taskId, TaskRequestDto taskDto)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId).ConfigureAwait(false);

        if (taskEntity == null)
            return null;

        var categoryExist = await _categoriesService.CategoryExistsAsync(taskDto.CategoryId);

        if (!categoryExist)
            throw new ArgumentException(ValidationMessages.CategoryDoesNotExist);

        taskEntity.Title = taskDto.Title;
        taskEntity.Description = taskDto.Description;
        taskEntity.DueDate = taskDto.DueDate;
        taskEntity.IsCompleted = taskDto.IsCompleted;
        taskEntity.CategoryId = taskDto.CategoryId;

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return taskEntity.ToOutDto();
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId);

        if (taskEntity == null)
            return false;

        _dbContext.Tasks.Remove(taskEntity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }
}
