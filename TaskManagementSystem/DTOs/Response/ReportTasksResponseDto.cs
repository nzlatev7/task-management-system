namespace TaskManagementSystem.DTOs.Response;

public sealed class ReportTasksResponseDto
{
    public int CategoryId { get; set; }

    public required IEnumerable<TaskResponseDto> Tasks { get; set; }
}
