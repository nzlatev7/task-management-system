namespace TaskManagementSystem.DTOs.Response;

public sealed class CategoryCompletionStatusResponseDto
{
    public short CompletionPercentage { get; set; }

    public required CompletionStatusStatsDto CompletionStatusStats { get; set; }
}