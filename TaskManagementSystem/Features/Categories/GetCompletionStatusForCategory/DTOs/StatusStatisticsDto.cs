namespace TaskManagementSystem.Features.Categories.DTOs;

public sealed class StatusStatisticsDto
{
    public int NumberOfPendingTasks { get; set; }

    public int NumberOfInProgressTasks { get; set; }

    public int NumberOfCompletedTasks { get; set; }

    public int NumberOfArchivedTasks { get; set; }

    public int NumberOfLockedTasks { get; set; }
}