using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Enums;

namespace TaskManagementSystem.Features.Tasks;

public class DeleteTaskLoggingHandler : IPipelineBehavior<DeleteTaskCommand, DeleteAction>
{
    private readonly ILogger<DeleteTaskLoggingHandler> _logger;

    public DeleteTaskLoggingHandler(ILogger<DeleteTaskLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<DeleteAction> Handle(
        DeleteTaskCommand request,
        RequestHandlerDelegate<DeleteAction> next,
        CancellationToken cancellationToken)
    {
        var deleteAction = await next();

        LogProperMessageForDelete(deleteAction, request.Id);

        return deleteAction;
    }

    private void LogProperMessageForDelete(DeleteAction deleteAction, int taskId)
    {
        switch (deleteAction)
        {
            case DeleteAction.Removed:
                _logger.LogInformation(LoggingMessageConstants.TaskRemovedSuccessfully, taskId);
                break;
            case DeleteAction.Moved:
                _logger.LogInformation(LoggingMessageConstants.TaskMovedSuccessfully, taskId);
                break;
            case DeleteAction.Locked:
                _logger.LogInformation(LoggingMessageConstants.TaskLockedSuccessfully, taskId);
                break;
            default:
                break;
        }
    }
}
