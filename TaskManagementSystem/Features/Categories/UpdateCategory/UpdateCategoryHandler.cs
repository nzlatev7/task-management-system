using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Categories.UpdateCategory.Extensions;

namespace TaskManagementSystem.Features.Categories;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public UpdateCategoryHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryResponseDto> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.Id)
            ?? throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        request.UpdateCategoryEntity(category);

        await _dbContext.SaveChangesAsync();

        return category.ToOutDto();
    }
}
