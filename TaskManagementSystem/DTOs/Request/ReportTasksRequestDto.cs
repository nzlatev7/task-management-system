using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public sealed class ReportTasksRequestDto
{
    public Status? Status { get; set; }

    public Priority? Priority { get; set; }

    public DateTime? DueBefore { get; set; }

    public DateTime? DueAfter { get; set; }
}
