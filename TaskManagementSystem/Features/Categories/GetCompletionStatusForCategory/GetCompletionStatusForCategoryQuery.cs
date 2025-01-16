using MediatR;
using TaskManagementSystem.Features.Categories.DTOs;

namespace TaskManagementSystem.Features.Categories;

public class GetCompletionStatusForCategoryQuery : IRequest<CategoryCompletionStatusResponseDto>
{
    public GetCompletionStatusForCategoryQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}