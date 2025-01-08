using MediatR;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Tasks;

public class GetTaskByIdQuery : IRequest<TaskResponseDto>
{
    public GetTaskByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}