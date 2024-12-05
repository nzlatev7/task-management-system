namespace TaskManagementSystem.DTOs.Response;

public sealed class CompletionStatusStatsDto
{
    public int PendingCount { get; set; }

    public int InProgressCount { get; set; }

    public int CompletedCount { get; set; }

    public int ArchivedCount { get; set; }
}