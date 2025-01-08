using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Tasks;

public class UpdateTaskLoggingHandler : IPipelineBehavior<UpdateTaskCommand, TaskResponseDto>
{
    private readonly ILogger<UpdateTaskLoggingHandler> _logger;

    public UpdateTaskLoggingHandler(ILogger<UpdateTaskLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<TaskResponseDto> Handle(
        UpdateTaskCommand request,
        RequestHandlerDelegate<TaskResponseDto> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        _logger.LogInformation(
            LoggingMessageConstants.TaskUpdatedSuccessfully,
            result.Id);

        return result;
    }
}
