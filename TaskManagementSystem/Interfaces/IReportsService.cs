using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface IReportsService
{
    Task<IEnumerable<ReportTasksResponseDto>> GetReportForTasksAsync(ReportTasksRequestDto reportFilters);
}
