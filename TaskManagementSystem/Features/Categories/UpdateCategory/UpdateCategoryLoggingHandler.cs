using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class UpdateCategoryLoggingHandler : IPipelineBehavior<UpdateCategoryCommand, CategoryResponseDto>
{
    private readonly ILogger<UpdateCategoryLoggingHandler> _logger;

    public UpdateCategoryLoggingHandler(ILogger<UpdateCategoryLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<CategoryResponseDto> Handle(
        UpdateCategoryCommand request,
        RequestHandlerDelegate<CategoryResponseDto> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        _logger.LogInformation(
            LoggingMessageConstants.CategoryUpdatedSuccessfully,
            result.Id);

        return result;
    }
}
