using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class CreateCategoryLoggingHandler : IPipelineBehavior<CreateCategoryCommand, CategoryResponseDto>
{
    private readonly ILogger<CreateCategoryLoggingHandler> _logger;

    public CreateCategoryLoggingHandler(ILogger<CreateCategoryLoggingHandler> logger)
    {
        _logger = logger;
    }

    public async Task<CategoryResponseDto> Handle(
        CreateCategoryCommand request,
        RequestHandlerDelegate<CategoryResponseDto> next,
        CancellationToken cancellationToken)
    {
        var result = await next();

        _logger.LogInformation(
            LoggingMessageConstants.CategoryCreatedSuccessfully,
            result.Id);

        return result;
    }
}