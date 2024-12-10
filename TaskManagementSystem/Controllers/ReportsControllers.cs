using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Controllers;

[ApiController]
public class ReportsControllers : ControllerBase
{
    private readonly IReportsService _reportsService;

    public ReportsControllers(IReportsService reportsService)
    {
        _reportsService = reportsService;
    }

    [HttpGet]
    [Route(RouteConstants.ReportForTasks)]
    public async Task<ActionResult<IEnumerable<ReportTasksResponseDto>>> GetReportForTasks([FromQuery] ReportTasksRequestDto reportDto)
    {
        var result = await _reportsService.GetReportForTasksAsync(reportDto);

        return Ok(result);
    }
}
