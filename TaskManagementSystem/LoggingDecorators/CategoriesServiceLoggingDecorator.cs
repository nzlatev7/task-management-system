using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.LoggingDecorators;

public class CategoriesServiceLoggingDecorator : ICategoriesService
{
    private readonly ICategoriesService _wrappee;
    private readonly ILogger<ICategoriesService> _logger;

    public CategoriesServiceLoggingDecorator(
        ICategoriesService wrapee,
        ILogger<ICategoriesService> logger)
    {
        _wrappee = wrapee;
        _logger = logger;
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto)
    {
        var category = await _wrappee.CreateCategoryAsync(categoryDto);

        _logger.LogInformation(LoggingMessageConstants.CategoryCreatedSuccessfully, category.Id);

        return category;
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        return await _wrappee.GetAllCategoriesAsync();
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId)
    {
        return await _wrappee.GetCategoryByIdAsync(categoryId);
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto)
    {
        var category = await _wrappee.UpdateCategoryAsync(categoryId, categoryDto);

        _logger.LogInformation(LoggingMessageConstants.CategoryUpdatedSuccessfully, categoryId);

        return category;
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        await _wrappee.DeleteCategoryAsync(categoryId);

        _logger.LogInformation(LoggingMessageConstants.CategoryDeletedSuccessfully, categoryId);
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksByCategoryAsync(int categoryId)
    {
        return await _wrappee.GetTasksByCategoryAsync(categoryId);
    }

    public async Task<CategoryCompletionStatusResponseDto> GetCompletionStatusForCategoryAsync(int categoryId)
    {
        return await _wrappee.GetCompletionStatusForCategoryAsync(categoryId);
    }
}
