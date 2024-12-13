using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;

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
        var category = new CategoryEntity()
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return category.ToOutDto();
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _dbContext.Categories
            .ToOutDtos();

        return categories;
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId);

        if (category is null)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        return category.ToOutDto();
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId);

        if (category is null)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;

        await _dbContext.SaveChangesAsync();

        return category.ToOutDto();
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var taskAssociatedToCategory = await _dbContext.Tasks
            .AnyAsync(x => x.CategoryId == categoryId);

        if (taskAssociatedToCategory)
            throw new ConflictException(ErrorMessageConstants.AssociatedTasksToCategory);

        var deletedRows = await _dbContext.Categories
            .Where(x => x.Id == categoryId)
            .ExecuteDeleteAsync();

        if (deletedRows is 0)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    public async Task<IEnumerable<TaskResponseDto>> GetTasksByCategoryAsync(int categoryId)
    {
        await ValidateCategory(categoryId);

        var tasks = await _dbContext.Tasks
            .Where(x => x.CategoryId == categoryId)
            .ToOutDtos();

        return tasks;
    }

    public async Task<CategoryCompletionStatusResponseDto> GetCompletionStatusForCategoryAwait(int categoryId)
    {
        await ValidateCategory(categoryId);

        var statusStatistics = await GetTaskStatusStatatisticsForCategory(categoryId);

        var completionPercentage = CalculateCompletionPercentage(statusStatistics);
            
        await _dbContext.Categories
            .Where(x => x.Id == categoryId)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.CompletionPercentage, completionPercentage));

        var result = new CategoryCompletionStatusResponseDto()
        {
            CompletionPercentage = completionPercentage,
            CompletionStatusStats = statusStatistics
        };

        return result;
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId);
    }

    private async Task ValidateCategory(int categoryId)
    {
        var exist = await CategoryExistsAsync(categoryId);
        if (exist is false)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private short CalculateCompletionPercentage(StatusStatisticsDto statusStatistics)
    {
        var validTasksCount = statusStatistics.NumberOfPendingTasks + statusStatistics.NumberOfInProgressTasks + statusStatistics.NumberOfCompletedTasks;
        if (validTasksCount == 0)
            throw new ConflictException(ErrorMessageConstants.CategoryWithoutTasks);

        short completionPercentage = (short)((statusStatistics.NumberOfCompletedTasks / (double)validTasksCount) * 100);

        return completionPercentage;
    }

    private async Task<StatusStatisticsDto> GetTaskStatusStatatisticsForCategory(int categoryId)
    {
        var taskStatusToCountForCategory = await _dbContext.Tasks
            .Where(t => t.CategoryId == categoryId)
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count);

        var result = new StatusStatisticsDto()
        {
            NumberOfPendingTasks = taskStatusToCountForCategory.GetValueOrDefault(Status.Pending, 0),
            NumberOfInProgressTasks = taskStatusToCountForCategory.GetValueOrDefault(Status.InProgress, 0),
            NumberOfCompletedTasks = taskStatusToCountForCategory.GetValueOrDefault(Status.Completed, 0),
            NumberOfArchivedTasks = taskStatusToCountForCategory.GetValueOrDefault(Status.Archived, 0)
        };

        return result;
    }
}