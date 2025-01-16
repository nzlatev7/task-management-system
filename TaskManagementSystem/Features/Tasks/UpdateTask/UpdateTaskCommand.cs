using MediatR;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Tasks;

public class UpdateTaskCommand : IRequest<TaskResponseDto>
{
    public UpdateTaskCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public Priority? Priority { get; set; }

    public Status Status { get; set; }

    public int CategoryId { get; set; }
}