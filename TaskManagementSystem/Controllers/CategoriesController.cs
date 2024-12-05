using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CategoryRequestDto categoryDto)
    {
        var result = await _categoriesService.CreateCategoryAsync(categoryDto);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategories()
    {
        var result = await _categoriesService.GetAllCategoriesAsync();

        return Ok(result);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<CategoryResponseDto>> GetCategoryById([FromRoute] int categoryId)
    {
        var result = await _categoriesService.GetCategoryByIdAsync(categoryId);

        return Ok(result);
    }

    [HttpPut("{categoryId}")]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory([FromRoute] int categoryId, [FromBody] CategoryRequestDto categoryDto)
    {
        var result = await _categoriesService.UpdateCategoryAsync(categoryId, categoryDto);

        return Ok(result);
    }


    [HttpDelete("{categoryId}")]
    public async Task<ActionResult> DeleteCategory([FromRoute] int categoryId)
    {
        await _categoriesService.DeleteCategoryAsync(categoryId);

        return Ok();
    }

    [HttpGet("{categoryId}/tasks")]
    public async Task<ActionResult<List<TaskResponseDto>>> GetTasksByCategory([FromRoute] int categoryId)
    {
        var result = await _categoriesService.GetTasksByCategoryAsync(categoryId);
        
        return Ok(result);
    }

    [HttpGet("{categoryId}/completion")]
    public async Task<ActionResult<CategoryCompletionStatusResponseDto>> GetCompletionStatus([FromRoute] int categoryId)
    {
        var result = await _categoriesService.GetCompletionStatus(categoryId);

        return Ok(result);
    }
}