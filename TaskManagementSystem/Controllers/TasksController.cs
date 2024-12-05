using Microsoft.AspNetCore.Mvc;
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
        var result = await _tasksService.CreateTaskAsync(taskDto);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks([FromQuery] bool sortByPriorityAscending)
    {
        var result = await _tasksService.GetAllTasksAsync(sortByPriorityAscending);

        return Ok(result);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById([FromRoute] int taskId)
    {
        var result = await _tasksService.GetTaskByIdAsync(taskId);

        return Ok(result);
    }

    [HttpPut("{taskId}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask([FromRoute] int taskId, [FromBody] TaskRequestDto taskDto)
    {
        var result = await _tasksService.UpdateTaskAsync(taskId, taskDto);

        return Ok(result);
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> DeleteTask([FromRoute] int taskId)
    {
        await _tasksService.DeleteTaskAsync(taskId);

        return Ok();
    }
}
