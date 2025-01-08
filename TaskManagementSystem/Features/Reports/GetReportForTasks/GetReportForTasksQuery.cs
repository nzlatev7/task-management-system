using MediatR;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Reports.DTOs;

namespace TaskManagementSystem.Features.Reports;

public class GetReportForTasksQuery : IRequest<IEnumerable<ReportTasksResponseDto>>
{
    public Status? Status { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? DueAfter { get; set; }

    public DateTime? DueBefore { get; set; }
}
