using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;

namespace TaskManagementSystem.Features.Tasks;

public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, DeleteAction>
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ITaskDeleteOrchestrator _taskDeleteOrchestrator;

    public DeleteTaskHandler(
        TaskManagementSystemDbContext dbContext,
        ITaskDeleteOrchestrator taskDeleteOrchestrator)
    {
        _dbContext = dbContext;
        _taskDeleteOrchestrator = taskDeleteOrchestrator;
    }

    public async Task<DeleteAction> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(request.Id)
        ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        ValidateTaskStatusForDeletion(taskEntity);

        var deleteAction = await _taskDeleteOrchestrator.ExecuteDeletionAsync(taskEntity, _dbContext);

        return deleteAction;
    }

    private static void ValidateTaskStatusForDeletion(TaskEntity taskEntity)
    {
        if (taskEntity.Status == Status.Locked)
            throw new ConflictException(ErrorMessageConstants.TaskAlreadyLocked);

        if (taskEntity.Status == Status.Archived)
            throw new ConflictException(ErrorMessageConstants.ArchivedTaskCanNotBeDeleted);
    }
}
