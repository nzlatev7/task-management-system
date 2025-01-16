using MediatR;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Features.Tasks;

public class UnlockTaskLoggingHandler : IPipelineBehavior<UnlockTaskCommand, Unit>
{
    private readonly ILogger<UnlockTaskLoggingHandler> _logger;

    public UnlockTaskLoggingHandler(ILogger<UnlockTaskLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Unit> Handle(
        UnlockTaskCommand request,
        RequestHandlerDelegate<Unit> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        _logger.LogInformation(
            LoggingMessageConstants.TaskUnlockedSuccessfully,
            request.Id);

        return result;
    }
}
