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
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryRequestDto categoryDto)
    {
        var result = await _categoriesService.CreateCategoryAsync(categoryDto);
        return new OkObjectResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetAllCategories()
    {
        var result = await _categoriesService.GetAllCategoriesAsync();
        return new OkObjectResult(result);
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<CategoryResponseDto>> GetCategoryById([FromRoute] int categoryId)
    {
        var result = await _categoriesService.GetCategoryByIdAsync(categoryId);

        if (result == null)
            return new NotFoundResult();

        return new OkObjectResult(result);
    }

    [HttpPut("{categoryId}")]
    public async Task<ActionResult<CategoryResponseDto>> UpdateCategory([FromRoute] int categoryId, CategoryRequestDto categoryDto)
    {
        var result = await _categoriesService.UpdateCategoryAsync(categoryId, categoryDto);

        if (result == null)
            return new NotFoundResult();

        return new OkObjectResult(result);
    }


    [HttpDelete("{categoryId}")]
    public async Task<ActionResult> DeleteCategory([FromRoute] int categoryId)
    {
        var result = await _categoriesService.DeleteCategoryAsync(categoryId);

        if (result == false)
            return new NotFoundResult();

        return new OkResult();
    }

    [HttpGet("{categoryId}/tasks")]
    public async Task<ActionResult<List<TaskResponseDto>>> GetTasksByCategory([FromRoute] int categoryId)
    {
        var category = await _categoriesService.GetCategoryByIdAsync(categoryId);
        
        if (category == null)
            return new NotFoundResult();

        var result = await _categoriesService.GetTasksByCategoryAsync(categoryId);
        
        return new OkObjectResult(result);
    }
}