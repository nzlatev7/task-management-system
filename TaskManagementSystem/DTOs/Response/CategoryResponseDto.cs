using System.Reflection.Metadata.Ecma335;

namespace TaskManagementSystem.DTOs.Response;

public sealed class CategoryResponseDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public List<TaskResponseDto>? Tasks{ get; set; }
}
