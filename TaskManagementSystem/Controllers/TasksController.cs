using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public TasksController(TaskManagementSystemDbContext context)
    {
        _dbContext = context;
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(TaskRequestDto taskDto)
    {
        var categoryExist = await CategoryExistsAsync(taskDto.CategoryId);

        if (!categoryExist)
            return new BadRequestObjectResult(ValidationMessages.CategoryDoesNotExist);

        var taskEntity = new TaskEntity()
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            IsCompleted = taskDto.IsCompleted,
            CategoryId = taskDto.CategoryId,
        };
        
        await _dbContext.Tasks.AddAsync(taskEntity).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        var result = taskEntity.ToOutDto();

        return new OkObjectResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetAllTasks()
    {
        var result = new List<TaskResponseDto>();

        var tasks = await _dbContext.Tasks.ToListAsync().ConfigureAwait(false);

        foreach (var taskEntity in tasks) 
        {
            result.Add(taskEntity.ToOutDto());
        }

        return new OkObjectResult(result);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId).ConfigureAwait(false);

        if (taskEntity == null)
            return new NotFoundResult();

        var result = taskEntity.ToOutDto();

        return new OkObjectResult(result);
    }


    [HttpPut("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask(int taskId, TaskRequestDto taskDto)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId).ConfigureAwait(false);

        if (taskEntity == null)
            return new NotFoundResult();

        var categoryExist = await CategoryExistsAsync(taskDto.CategoryId);

        if (!categoryExist)
            return new BadRequestObjectResult(ValidationMessages.CategoryDoesNotExist);

        taskEntity.Title = taskDto.Title;
        taskEntity.Description = taskDto.Description;
        taskEntity.DueDate = taskDto.DueDate;
        taskEntity.IsCompleted = taskDto.IsCompleted;
        taskEntity.CategoryId = taskDto.CategoryId;

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        var result = taskEntity.ToOutDto();

        return new OkObjectResult(taskDto);
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> DeleteTask(int taskId)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(taskId).ConfigureAwait(false);

        if (taskEntity == null)
            return new NotFoundResult();

        _dbContext.Tasks.Remove(taskEntity);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return new OkResult();
    }

    private async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId).ConfigureAwait(false);
    }
}
