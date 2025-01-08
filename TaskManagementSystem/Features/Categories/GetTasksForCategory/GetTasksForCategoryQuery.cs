using MediatR;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Categories;

public class GetTasksForCategoryQuery : IRequest<IEnumerable<TaskResponseDto>>
{
    public GetTasksForCategoryQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}