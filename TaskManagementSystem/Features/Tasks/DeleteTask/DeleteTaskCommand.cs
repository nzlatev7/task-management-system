using MediatR;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Tasks;

public class DeleteTaskCommand : IRequest<DeleteAction>
{
    public DeleteTaskCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}