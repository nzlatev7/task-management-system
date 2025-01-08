using MediatR;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Tasks;

public class UnlockTaskCommand : IRequest
{
    public UnlockTaskCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

    public Status Status { get; set; }
}
