using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Categories.Shared;

public sealed class CategoryResponseDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public List<TaskResponseDto>? Tasks { get; set; }
}
