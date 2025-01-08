using MediatR;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Tasks;

public class CreateTaskCommand : IRequest<TaskResponseDto>
{
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public Priority? Priority { get; set; }

    public int CategoryId { get; set; }
}