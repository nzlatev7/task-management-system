using MediatR;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class GetCategoryByIdQuery : IRequest<CategoryResponseDto>
{
    public GetCategoryByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}
