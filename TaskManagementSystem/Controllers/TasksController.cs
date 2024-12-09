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
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks([FromQuery] bool sortByPriorityAscending)
    {
        var result = await _tasksService.GetAllTasksAsync(sortByPriorityAscending);

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

    [HttpDelete]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult> DeleteTask([FromRoute] int id)
    {
        await _tasksService.DeleteTaskAsync(id);

        return Ok();
    }
}
