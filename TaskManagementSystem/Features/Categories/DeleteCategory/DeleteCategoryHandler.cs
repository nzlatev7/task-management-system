using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Exceptions;

namespace TaskManagementSystem.Features.Categories;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public DeleteCategoryHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var taskAssociatedToCategory = await _dbContext.Tasks
            .AnyAsync(x => x.CategoryId == request.Id);

        if (taskAssociatedToCategory)
            throw new ConflictException(ErrorMessageConstants.AssociatedTasksToCategory);

        var deletedRows = await _dbContext.Categories
            .Where(x => x.Id == request.Id)
            .ExecuteDeleteAsync();

        if (deletedRows is 0)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);
    }
}
