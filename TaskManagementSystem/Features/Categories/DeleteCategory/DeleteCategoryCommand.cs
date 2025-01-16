using MediatR;

namespace TaskManagementSystem.Features.Categories;

public class DeleteCategoryCommand : IRequest
{
    public DeleteCategoryCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}
