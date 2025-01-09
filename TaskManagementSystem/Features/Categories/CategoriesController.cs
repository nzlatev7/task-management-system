using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Categories.DTOs;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Categories;

[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route(RouteConstants.Categories)]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CategoryRequestDto categoryDto)
    {
        var command = categoryDto.Adapt<CreateCategoryCommand>();

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.Categories)]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.CategoryById)]
    public async Task<ActionResult<CategoryResponseDto>> GetCategoryById([FromRoute] int id)
    {
        var query = new GetCategoryByIdQuery(id);

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut]
    [Route(RouteConstants.CategoryById)]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory([FromRoute] int id, [FromBody] CategoryRequestDto categoryDto)
    {
        var command = new UpdateCategoryCommand(id);
        command = categoryDto.Adapt(command);

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpDelete]
    [Route(RouteConstants.CategoryById)]
    public async Task<ActionResult> DeleteCategory([FromRoute] int id)
    {
        var command = new DeleteCategoryCommand(id);

        await _mediator.Send(command);

        return Ok();
    }

    [HttpGet]
    [Route(RouteConstants.TasksByCategory)]
    public async Task<ActionResult<List<TaskResponseDto>>> GetTasksByCategory([FromRoute] int id)
    {
        var query = new GetTasksForCategoryQuery(id);

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.CompletionStatus)]
    public async Task<ActionResult<CategoryCompletionStatusResponseDto>> GetCompletionStatusForCategory([FromRoute] int id)
    {
        var query = new GetCompletionStatusForCategoryQuery(id);

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}