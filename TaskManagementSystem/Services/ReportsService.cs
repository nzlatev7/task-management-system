using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.Interfaces.Reports;

namespace TaskManagementSystem.Services;

public class ReportsService : IReportsService
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly IReportTasksQueryBuilder _reportTasksQueryBuilder;

    public ReportsService(TaskManagementSystemDbContext dbContext, IReportTasksQueryBuilder reportTasksQueryBuilder)
    {
        _dbContext = dbContext;
        _reportTasksQueryBuilder = reportTasksQueryBuilder;
    }

    public async Task<IEnumerable<ReportTasksResponseDto>> GetReportForTasksAsync(ReportTasksRequestDto reportFilters)
    {
        IQueryable<TaskEntity> tasks = _reportTasksQueryBuilder.Apply(_dbContext.Tasks, reportFilters);

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

}