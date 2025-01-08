using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Reports;
using TaskManagementSystem.Features.Reports.DTOs;

namespace TaskManagementSystem.Controllers;

[ApiController]
public class ReportsControllers : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsControllers(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route(RouteConstants.ReportForTasks)]
    public async Task<ActionResult<IEnumerable<ReportTasksResponseDto>>> GetReportForTasks([FromQuery] ReportTasksRequestDto reportFilters)
    {
        var query = reportFilters.Adapt<GetReportForTasksQuery>();

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
