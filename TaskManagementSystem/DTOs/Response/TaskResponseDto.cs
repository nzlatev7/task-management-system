namespace TaskManagementSystem.DTOs.Response;

public sealed class TaskResponseDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public int CategoryId { get; set; }
}