using TaskManagementSystem.Enums;

namespace TaskManagementSystem.DTOs.Request;

public sealed class CloneTaskRequestDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public int? CategoryId { get; set; }

    public Priority? Priority { get; set; }

    public Status? Status { get; set; }

    public int? StoryPoints { get; set; }

    public int? Severity { get; set; }
}
