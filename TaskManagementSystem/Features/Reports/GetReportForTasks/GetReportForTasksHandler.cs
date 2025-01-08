using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Features.Reports.DTOs;

namespace TaskManagementSystem.Features.Reports;

public class GetReportForTasksHandler 
    : IRequestHandler<GetReportForTasksQuery, IEnumerable<ReportTasksResponseDto>>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public GetReportForTasksHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ReportTasksResponseDto>> Handle(
        GetReportForTasksQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<TaskEntity> tasks = ApplyFilters(_dbContext.Tasks, request);

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

    private IQueryable<TaskEntity> ApplyFilters(IQueryable<TaskEntity> tasks, GetReportForTasksQuery reportDto)
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
