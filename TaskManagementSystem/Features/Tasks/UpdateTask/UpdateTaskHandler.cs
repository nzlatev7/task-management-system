using MediatR;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Tasks.UpdateTask.Extensions;

namespace TaskManagementSystem.Features.Tasks;

public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryChecker _categoryChecker;

    public UpdateTaskHandler(TaskManagementSystemDbContext dbContext, ICategoryChecker categoryChecker)
    {
        _dbContext = dbContext;
        _categoryChecker = categoryChecker;
    }

    public async Task<TaskResponseDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(request.Id)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        ValidateTaskStatusForUpdateAsync(taskEntity, request);

        await VerifyCategoryExistsAsync(taskEntity.CategoryId);

        request.UpdateTaskEntity(taskEntity);

        await _dbContext.SaveChangesAsync();

        return taskEntity.ToOutDto();
    }

    private async Task VerifyCategoryExistsAsync(int categoryId)
    {
        var categoryExists = await _categoryChecker.CategoryExistsAsync(categoryId);
        if (!categoryExists)
            throw new BadHttpRequestException(ErrorMessageConstants.CategoryDoesNotExist);
    }

    private void ValidateTaskStatusForUpdateAsync(TaskEntity taskEntity, UpdateTaskCommand request)
    {
        var canArchivedBeEdited = taskEntity.Status == Status.Archived;
        if (canArchivedBeEdited)
            throw new ConflictException(ErrorMessageConstants.ArchivedTaskCanNotBeEdited);

        var canArchiveEntity = taskEntity.Status != Status.Completed && request.Status == Status.Archived;
        if (canArchiveEntity)
            throw new ConflictException(ErrorMessageConstants.OnlyCompletedTaskCanBeArchived);

        var canLockedBeEdited = taskEntity.Status == Status.Locked;
        if (canLockedBeEdited)
            throw new ConflictException(ErrorMessageConstants.LockedTaskCanNotBeEdited);
    }
}