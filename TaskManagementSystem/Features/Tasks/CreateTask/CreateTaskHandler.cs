using MediatR;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Shared.Extensions;
using TaskManagementSystem.Features.Tasks.CreateTask.Extensions;

namespace TaskManagementSystem.Features.Tasks;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryChecker _categoryChecker;

    public CreateTaskHandler(
        TaskManagementSystemDbContext dbContext,
        ICategoryChecker categoryChecker)
    {
        _dbContext = dbContext;
        _categoryChecker = categoryChecker;
    }

    public async Task<TaskResponseDto> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        await VerifyCategoryExistsAsync(request.CategoryId);

        var taskEntity = request.ToTaskEntityForCreate();

        await _dbContext.Tasks.AddAsync(taskEntity);
        await _dbContext.SaveChangesAsync();

        return taskEntity.ToOutDto();
    }

    private async Task VerifyCategoryExistsAsync(int categoryId)
    {
        var categoryExists = await _categoryChecker.CategoryExistsAsync(categoryId);
        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);
    }
}
