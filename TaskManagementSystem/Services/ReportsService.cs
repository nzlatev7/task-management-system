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

    public async Task<IEnumerable<ReportTasksResponseDto>> GetReportForTasksAsync(ReportTasksRequestDto reportDto)
    {
        IQueryable<TaskEntity> tasks = _dbContext.Tasks;

        tasks = ApplyFilters(tasks, reportDto);

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

    private IQueryable<TaskEntity> ApplyFilters(IQueryable<TaskEntity> tasks, ReportTasksRequestDto reportDto)
    {
        tasks = ApplyStatusFilter(tasks, reportDto);

        tasks = ApplyPriorityFilter(tasks, reportDto);

        tasks = ApplyDatesFilter(tasks, reportDto);

        return tasks;
    }

    private IQueryable<TaskEntity> ApplyStatusFilter(IQueryable<TaskEntity> tasks, ReportTasksRequestDto reportDto)
    {
        if (reportDto.Status.HasValue)
            tasks = tasks.Where(x => x.Status == reportDto.Status);

        return tasks;
    }

    private IQueryable<TaskEntity> ApplyPriorityFilter(IQueryable<TaskEntity> tasks, ReportTasksRequestDto reportDto)
    {
        if (reportDto.Priority.HasValue)
            tasks = tasks.Where(x => x.Priority == reportDto.Priority);

        return tasks;
    }

    private IQueryable<TaskEntity> ApplyDatesFilter(IQueryable<TaskEntity> tasks, ReportTasksRequestDto reportDto)
    {
        if (reportDto.DueBefore.HasValue && reportDto.DueAfter.HasValue)
            tasks = tasks.Where(x => x.DueDate < reportDto.DueBefore || x.DueDate > reportDto.DueAfter);
        else if (reportDto.DueBefore.HasValue)
            tasks = tasks.Where(x => x.DueDate < reportDto.DueBefore);
        else if (reportDto.DueAfter.HasValue)
            tasks = tasks.Where(x => x.DueDate > reportDto.DueBefore);

        return tasks;
    }
}