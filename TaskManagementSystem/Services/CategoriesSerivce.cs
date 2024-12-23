using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Services;

public sealed class CategoriesSerivce : ICategoriesService
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesSerivce(
        TaskManagementSystemDbContext dbContext,
        ICategoryRepository categoryRepository)
    {
        _dbContext = dbContext;
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto categoryDto)
    {
        var category = categoryDto.ToCategoryEntityForCreate();

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return category.ToOutDto();
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _dbContext.Categories
            .Select(x => new CategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();

        return categories;
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(int categoryId)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId)
            ?? throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        return category.ToOutDto();
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto)
    {
        var category = await _dbContext.Categories.FindAsync(categoryId)
            ?? throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        categoryDto.UpdateCategoryEntity(category);

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
        await ValidateCategoryAsync(categoryId);

        var tasks = await _dbContext.Tasks
            .Where(x => x.CategoryId == categoryId)
            .Select(x => new TaskResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DueDate = x.DueDate,
                Priority = x.Priority,
                IsCompleted = x.IsCompleted,
                Status = x.Status,
                CategoryId = x.CategoryId
            }).ToListAsync();

        return tasks;
    }

    public async Task<CategoryCompletionStatusResponseDto> GetCompletionStatusForCategoryAsync(int categoryId)
    {
        await ValidateCategoryAsync(categoryId);

        var statusStatistics = await GetTaskStatusStatatisticsForCategoryAsync(categoryId);

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

    private async Task ValidateCategoryAsync(int categoryId)
    {
        var exist = await _categoryRepository.CategoryExistsAsync(categoryId);
        if (exist is false)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private short CalculateCompletionPercentage(StatusStatisticsDto statusStatistics)
    {
        short completionPercentage;

        var validTasksCount = statusStatistics.NumberOfPendingTasks + statusStatistics.NumberOfInProgressTasks + statusStatistics.NumberOfCompletedTasks;
        if (validTasksCount == 0)
            completionPercentage = 0; // as there's nothing to measure progress against, so completion percentage is 0%

        completionPercentage = (short)((statusStatistics.NumberOfCompletedTasks / (double)validTasksCount) * 100);

        return completionPercentage;
    }

    private async Task<StatusStatisticsDto> GetTaskStatusStatatisticsForCategoryAsync(int categoryId)
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
            NumberOfArchivedTasks = taskStatusToCountForCategory.GetValueOrDefault(Status.Archived, 0),
            NumberOfLockedTasks = taskStatusToCountForCategory.GetValueOrDefault(Status.Locked, 0)
        };

        return result;
    }
}