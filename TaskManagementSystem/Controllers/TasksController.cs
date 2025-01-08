using Mapster;
using MediatR;
using TaskManagementSystem.Enums;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Features.Tasks.DTOs;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Controllers;

[ApiController]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route(RouteConstants.Tasks)]
    public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskRequestDto taskDto)
    {
        var command = taskDto.Adapt<CreateTaskCommand>();

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.Tasks)]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks([FromQuery] GetAllTasksRequestDto sortByDto)
    {
        var query = sortByDto.Adapt<GetAllTasksQuery>();

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById([FromRoute] int id)
    {
        var query = new GetTaskByIdQuery(id);

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask([FromRoute] int id, [FromBody] UpdateTaskRequestDto taskDto)
    {
        var command = new UpdateTaskCommand(id);
        command = taskDto.Adapt(command);

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPatch]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult> UnlockTask([FromRoute] int id, [FromBody] UnlockTaskRequestDto unlockDto)
    {
        var command = new UnlockTaskCommand(id);
        command = unlockDto.Adapt(command);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete]
    [Route(RouteConstants.TaskById)]
    public async Task<ActionResult<DeleteAction>> DeleteTask([FromRoute] int id)
    {
        var command = new DeleteTaskCommand(id);

        var deleteAction = await _mediator.Send(command);

        return Ok(deleteAction);
    }
}
