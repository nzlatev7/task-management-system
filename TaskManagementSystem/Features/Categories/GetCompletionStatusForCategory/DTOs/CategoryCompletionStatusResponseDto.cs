namespace TaskManagementSystem.Features.Categories.DTOs;

public sealed class CategoryCompletionStatusResponseDto
{
    public short CompletionPercentage { get; set; }

    public required StatusStatisticsDto CompletionStatusStats { get; set; }
}