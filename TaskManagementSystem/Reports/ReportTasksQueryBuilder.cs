using System.Linq;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.Interfaces.Reports;

namespace TaskManagementSystem.Reports;

public sealed class ReportTasksQueryBuilder : IReportTasksQueryBuilder
{
    public IQueryable<TaskEntity> Apply(IQueryable<TaskEntity> source, ReportTasksRequestDto filters)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(filters);

        var query = source;

        if (filters.Status.HasValue)
        {
            query = query.Where(task => task.Status == filters.Status);
        }

        if (filters.Priority.HasValue)
        {
            query = query.Where(task => task.Priority == filters.Priority);
        }

        if (filters.DueAfter.HasValue)
        {
            query = query.Where(task => task.DueDate > filters.DueAfter);
        }

        if (filters.DueBefore.HasValue)
        {
            query = query.Where(task => task.DueDate < filters.DueBefore);
        }

        return query;
    }
}
