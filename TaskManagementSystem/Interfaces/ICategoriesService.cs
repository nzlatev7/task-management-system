using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface ICategoriesService
{
    Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto);

    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();

    Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId);

    Task<CategoryResponseDto?> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto);

    Task<bool> DeleteCategoryAsync(int categoryId);

    Task<List<TaskResponseDto>> GetTasksByCategoryAsync(int categoryId);

    Task<bool> CategoryExistsAsync(int categoryId);
}