using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Categories.DTOs;

namespace TaskManagementSystem.Features.Categories;

public class GetCompletionStatusForCategoryHandler 
    : IRequestHandler<GetCompletionStatusForCategoryQuery, CategoryCompletionStatusResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryChecker _categoryChecker;

    public GetCompletionStatusForCategoryHandler(
        TaskManagementSystemDbContext dbContext,
        ICategoryChecker categoryChecker)
    {
        _dbContext = dbContext;
        _categoryChecker = categoryChecker;
    }

    public async Task<CategoryCompletionStatusResponseDto> Handle(
        GetCompletionStatusForCategoryQuery request,
        CancellationToken cancellationToken)
    {
        await VerifyCategoryExistsAsync(request.Id);

        var statusStatistics = await GetTaskStatusStatatisticsForCategoryAsync(request.Id);

        var completionPercentage = CalculateCompletionPercentage(statusStatistics);

        await _dbContext.Categories
            .Where(x => x.Id == request.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.CompletionPercentage, completionPercentage));

        var result = new CategoryCompletionStatusResponseDto()
        {
            CompletionPercentage = completionPercentage,
            CompletionStatusStats = statusStatistics
        };

        return result;
    }

    private async Task VerifyCategoryExistsAsync(int categoryId)
    {
        var exist = await _categoryChecker.CategoryExistsAsync(categoryId);
        if (exist is false)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private short CalculateCompletionPercentage(StatusStatisticsDto statusStatistics)
    {
        short completionPercentage;

        var validTasksCount = statusStatistics.NumberOfPendingTasks + statusStatistics.NumberOfInProgressTasks + statusStatistics.NumberOfCompletedTasks;
        if (validTasksCount == 0)
        {
            completionPercentage = 0; // as there's nothing to measure progress against, so completion percentage is 0%
        }
        else
        {
            completionPercentage = (short)((statusStatistics.NumberOfCompletedTasks / (double)validTasksCount) * 100);
        }

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
