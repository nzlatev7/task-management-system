using MediatR;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Tasks;

public class GetAllTasksQuery : IRequest<IEnumerable<TaskResponseDto>>
{
    public SortingTaskProperty Property { get; set; }

    public bool IsAscending { get; set; }
}