using MediatR;
using TaskManagementSystem.Constants;

namespace TaskManagementSystem.Features.Categories;

public class DeleteCategoryLoggingHandler : IPipelineBehavior<DeleteCategoryCommand, Unit>
{
    private readonly ILogger<DeleteCategoryLoggingHandler> _logger;

    public DeleteCategoryLoggingHandler(ILogger<DeleteCategoryLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<Unit> Handle(
        DeleteCategoryCommand request,
        RequestHandlerDelegate<Unit> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        _logger.LogInformation(
            LoggingMessageConstants.CategoryDeletedSuccessfully,
            request.Id);

        return result;
    }
}
