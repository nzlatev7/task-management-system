using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpPost]
    [Route(RouteConstants.Categories)]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CategoryRequestDto categoryDto)
    {
        var result = await _categoriesService.CreateCategoryAsync(categoryDto);

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.Categories)]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategories()
    {
        var result = await _categoriesService.GetAllCategoriesAsync();

        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.CategoryById)]
    public async Task<ActionResult<CategoryResponseDto>> GetCategoryById([FromRoute] int id)
    {
        var result = await _categoriesService.GetCategoryByIdAsync(id);

        return Ok(result);
    }

    [HttpPut]
    [Route(RouteConstants.CategoryById)]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory([FromRoute] int id, [FromBody] CategoryRequestDto categoryDto)
    {
        var result = await _categoriesService.UpdateCategoryAsync(id, categoryDto);

        return Ok(result);
    }

    [HttpDelete]
    [Route(RouteConstants.CategoryById)]
    public async Task<ActionResult> DeleteCategory([FromRoute] int id)
    {
        await _categoriesService.DeleteCategoryAsync(id);

        return Ok();
    }

    [HttpGet]
    [Route(RouteConstants.TasksByCategory)]
    public async Task<ActionResult<List<TaskResponseDto>>> GetTasksByCategory([FromRoute] int id)
    {
        var result = await _categoriesService.GetTasksByCategoryAsync(id);
        
        return Ok(result);
    }

    [HttpGet]
    [Route(RouteConstants.CompletionStatus)]
    public async Task<ActionResult<CategoryCompletionStatusResponseDto>> GetCompletionStatusForCategory([FromRoute] int id)
    {
        var result = await _categoriesService.GetCompletionStatusForCategoryAsync(id);

        return Ok(result);
    }
}