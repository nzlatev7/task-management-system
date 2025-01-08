using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Features.Tasks;

public class CreateTaskLoggingHandler : IPipelineBehavior<CreateTaskCommand, TaskResponseDto>
{
    private readonly ILogger<CreateTaskLoggingHandler> _logger;

    public CreateTaskLoggingHandler(ILogger<CreateTaskLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<TaskResponseDto> Handle(
        CreateTaskCommand request,
        RequestHandlerDelegate<TaskResponseDto> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        _logger.LogInformation(
             LoggingMessageConstants.TaskCreatedSuccessfully,
             response.Id,
             response.CategoryId);

        return response;
    }
}