using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;

namespace TaskManagementSystem.Features.Tasks;

public class UnlockTaskHandler : IRequestHandler<UnlockTaskCommand>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public UnlockTaskHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UnlockTaskCommand request, CancellationToken cancellationToken)
    {
        var updatedRows = await _dbContext.Tasks
            .Where(x => x.Id == request.Id && x.Status == Status.Locked)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Status, request.Status));

        if (updatedRows is 0)
            throw new NotFoundException(ErrorMessageConstants.LockedTaskWithIdDoesNotExist);
    }
}
