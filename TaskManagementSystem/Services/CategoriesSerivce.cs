using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services;

public sealed class CategoriesSerivce : ICategoriesService
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public CategoriesSerivce(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto)
    {
        var category = new Category()
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };

        await _dbContext.Categories.AddAsync(category).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        var result = category.ToOutDto();

        return result;
    }

    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var result = new List<CategoryResponseDto>();

        var categories = await _dbContext.Categories.ToListAsync().ConfigureAwait(false);

        foreach (var category in categories)
        {
            result.Add(category.ToOutDto());
        }

        return result;
    }

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(int categoryId)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId).ConfigureAwait(false);

        var result = category?.ToOutDto();

        return result;
    }

    public async Task<CategoryResponseDto?> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId).ConfigureAwait(false);

        if (category == null)
            return null;

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return category.ToOutDto();
    }

    public async Task<bool> DeleteCategoryAsync([FromRoute] int categoryId)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId).ConfigureAwait(false);

        if (category == null)
            return false;

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public async Task<List<TaskResponseDto>> GetTasksByCategoryAsync(int categoryId)
    {
        var tasks = await _dbContext.Tasks
            .Where(x => x.CategoryId == categoryId)
            .ToListAsync()
            .ConfigureAwait(false);

        var result = new List<TaskResponseDto>();

        foreach (var task in tasks)
        {
            result.Add(task.ToOutDto());
        }

        return result;
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId).ConfigureAwait(false);
    }
}
