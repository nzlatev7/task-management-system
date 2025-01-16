using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Shared.Extensions;

namespace TaskManagementSystem.Features.Categories;

public class GetTasksForCategoryHandler 
    : IRequestHandler<GetTasksForCategoryQuery, IEnumerable<TaskResponseDto>>
{
    private readonly TaskManagementSystemDbContext _dbContext;
    private readonly ICategoryChecker _categoryChecker;

    public GetTasksForCategoryHandler(
        TaskManagementSystemDbContext dbContext,
        ICategoryChecker categoryChecker)
    {
        _dbContext = dbContext;
        _categoryChecker = categoryChecker;
    }

    public async Task<IEnumerable<TaskResponseDto>> Handle(
        GetTasksForCategoryQuery request,
        CancellationToken cancellationToken)
    {
        await VerifyCategoryExistsAsync(request.Id);

        var tasks = await _dbContext.Tasks
            .Where(x => x.CategoryId == request.Id)
            .Select(x => x.ToOutDto())
            .ToListAsync();

        return tasks;
    }

    private async Task VerifyCategoryExistsAsync(int categoryId)
    {
        var exist = await _categoryChecker.CategoryExistsAsync(categoryId);
        if (exist is false)
            throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);
    }
}
