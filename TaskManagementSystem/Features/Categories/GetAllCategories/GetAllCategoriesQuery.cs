using MediatR;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class GetAllCategoriesQuery : IRequest<IEnumerable<CategoryResponseDto>>
{
}