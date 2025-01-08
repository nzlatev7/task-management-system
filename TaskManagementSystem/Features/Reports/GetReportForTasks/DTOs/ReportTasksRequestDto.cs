using TaskManagementSystem.Attributes;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Reports.DTOs;

public sealed class ReportTasksRequestDto
{
    public Status? Status { get; set; }

    public Priority? Priority { get; set; }

    [EnsureUtc]
    public DateTime? DueAfter { get; set; }

    [EnsureUtc]
    [EnsureDueBeforeIsLaterThanDueAfter]
    public DateTime? DueBefore { get; set; }
}
