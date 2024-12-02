using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public TasksController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] TaskRequestDto taskDto)
    {
        try
        {
            var result = await _tasksService.CreateTaskAsync(taskDto);
            return new OkObjectResult(result);
        }
        catch (ArgumentException ex) when (ex.Message == ValidationMessages.CategoryDoesNotExist)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetAllTasks()
    {
        var result = await _tasksService.GetAllTasksAsync();
        return new OkObjectResult(result);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById([FromRoute] int taskId)
    {
        var result = await _tasksService.GetTaskByIdAsync(taskId);

        if (result == null)
            return new NotFoundResult();

        return new OkObjectResult(result);
    }


    [HttpPut("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask([FromRoute] int taskId, [FromBody] TaskRequestDto taskDto)
    {
        try
        {
            var result = await _tasksService.UpdateTaskAsync(taskId, taskDto);

            if (result == null)
                return new NotFoundResult();

            return new OkObjectResult(result);
        }
        catch (ArgumentException ex) when (ex.Message == ValidationMessages.CategoryDoesNotExist)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> DeleteTask([FromRoute] int taskId)
    {
        var result = await _tasksService.DeleteTaskAsync(taskId);

        if (result == false)
            return new NotFoundResult();

        return new OkResult();
    }
}
