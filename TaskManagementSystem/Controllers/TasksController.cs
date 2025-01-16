using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public TasksController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpPost]
    [Route(RouteConstants.Tasks)]
    public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskRequestDto taskDto)
    {
        var result = await _tasksService.CreateTaskAsync(taskDto);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.Tasks)]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks([FromQuery] GetAllTasksRequestDto sortBy)
    {
        var result = await _tasksService.GetAllTasksAsync(sortBy);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById([FromRoute] int id)
    {
        var result = await _tasksService.GetTaskByIdAsync(id);

        return Ok(result);
    }

    [HttpPut]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto taskDto)
    {
        var result = await _tasksService.UpdateTaskAsync(id, taskDto);

        return Ok(result);
    }

    [HttpPatch]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult> UnlockTask([FromRoute] int id, [FromBody] UnlockTaskRequestDto unlockDto)
    {
        await _tasksService.UnlockTaskAsync(id, unlockDto);
        
        return NoContent();
    }

    [HttpDelete]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult> DeleteTask([FromRoute] int id)
    {
        var deleteAction = await _tasksService.DeleteTaskAsync(id);

        return Ok(deleteAction);
    }
}
