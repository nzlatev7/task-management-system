using MediatR;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class CreateCategoryCommand : IRequest<CategoryResponseDto>
{
    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}