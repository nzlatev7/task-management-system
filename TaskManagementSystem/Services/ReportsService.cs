using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Services;

public class ReportsService : IReportsService
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public ReportsService(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ReportTasksResponseDto>> GetReportForTasksAsync(ReportTasksRequestDto reportFilters)
    {
        IQueryable<TaskEntity> tasks = _dbContext.Tasks;

        tasks = GetTasksWithAppliedFilters(tasks, reportFilters);

        var result = await tasks
            .GroupBy(x => x.CategoryId)
            .Select(g => new ReportTasksResponseDto()
            {
                CategoryId = g.Key,
                Tasks = g.Select(x => x.ToOutDto())
                    .ToList()
            })
            .ToListAsync();

        return result;
    }

    private IQueryable<TaskEntity> GetTasksWithAppliedFilters(IQueryable<TaskEntity> tasks, ReportTasksRequestDto reportDto)
    {
        if (reportDto.Status.HasValue)
        {
            tasks = tasks.Where(x => x.Status == reportDto.Status);
        }

        if (reportDto.Priority.HasValue)
        {
            tasks = tasks.Where(x => x.Priority == reportDto.Priority);
        }

        if (reportDto.DueAfter.HasValue)
        {
            tasks = tasks.Where(x => x.DueDate > reportDto.DueAfter);
        }

        if (reportDto.DueBefore.HasValue)
        {
            tasks = tasks.Where(x => x.DueDate < reportDto.DueBefore);
        }

        return tasks;
    }
}