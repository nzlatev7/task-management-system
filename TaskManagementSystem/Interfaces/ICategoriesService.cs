using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface ICategoriesService
{
    Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto);

    Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();

    Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId);

    Task<CategoryResponseDto> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto);

    Task DeleteCategoryAsync(int categoryId);

    Task<IEnumerable<TaskResponseDto>> GetTasksByCategoryAsync(int categoryId);

    Task<CategoryCompletionStatusResponseDto> GetCompletionStatusForCategoryAsync(int categoryId);
}