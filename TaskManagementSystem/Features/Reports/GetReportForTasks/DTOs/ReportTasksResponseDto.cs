using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Reports.DTOs;

public sealed class ReportTasksResponseDto
{
    public int CategoryId { get; set; }

    public required IEnumerable<TaskResponseDto> Tasks { get; set; }
}
