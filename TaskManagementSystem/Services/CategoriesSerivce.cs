﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<CategoryResponseDto?> UpdateCategoryAsync(int categoryId, CategoryRequestDto categoryDto)
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
        var categoryExists = await CategoryExistsAsync(categoryId);

        if (!categoryExists)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        var tasks = await _dbContext.Tasks
            .Where(x => x.CategoryId == categoryId)
            .ToOutDtos();

        return tasks;
    }

    public async Task<CategoryCompletionStatusResponseDto> GetCompletionStatus(int categoryId)
    {
        var categoryStats = await _dbContext.Categories
            .Where(x => x.Id == categoryId)
            .Select(x => new CompletionStatusStatsDto()
            {
                PendingCount = x.Tasks.Where(x => x.Status == Status.Pending).Count(),
                InProgressCount = x.Tasks.Where(x => x.Status == Status.InProgress).Count(),
                CompletedCount = x.Tasks.Where(x => x.Status == Status.Completed).Count(),
                ArchivedCount = x.Tasks.Where(x => x.Status == Status.Archived).Count()
            }).FirstOrDefaultAsync();

        if (categoryStats == null)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        var validTasksCount = categoryStats.PendingCount + categoryStats.InProgressCount + categoryStats.CompletedCount;
        if (validTasksCount == 0)
            throw new ConflictException(ErrorMessageConstants.CategoryWithoutTasks);

        short completionPercentage = (short)((categoryStats.CompletedCount / (double)validTasksCount) * 100);

        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE category SET completion_percentage = {completionPercentage} WHERE id = {categoryId}");
        
        var result = new CategoryCompletionStatusResponseDto()
        {
            CompletionPercentage = completionPercentage,
            CompletionStatusStats = categoryStats,
        };

        return result;
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId);
    }
}