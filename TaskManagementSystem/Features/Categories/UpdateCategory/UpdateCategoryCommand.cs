using MediatR;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class UpdateCategoryCommand : IRequest<CategoryResponseDto>
{
    public UpdateCategoryCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}
