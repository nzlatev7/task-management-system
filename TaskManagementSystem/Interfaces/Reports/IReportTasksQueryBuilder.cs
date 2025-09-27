using System.Linq;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;

namespace TaskManagementSystem.Interfaces.Reports;

public interface IReportTasksQueryBuilder
{
    IQueryable<TaskEntity> Apply(IQueryable<TaskEntity> source, ReportTasksRequestDto filters);
}
