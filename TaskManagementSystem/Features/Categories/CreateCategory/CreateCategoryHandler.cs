using MediatR;
using TaskManagementSystem.Database;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Categories.CreateCategory.Extensions;
using TaskManagementSystem.Features.Shared.Extensions;

namespace TaskManagementSystem.Features.Categories;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public CreateCategoryHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryResponseDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = request.ToCategoryEntityForCreate();

        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return category.ToOutDto();
    }
}