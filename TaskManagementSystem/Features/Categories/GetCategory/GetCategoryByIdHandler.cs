using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Shared.Extensions;

namespace TaskManagementSystem.Features.Categories;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public GetCategoryByIdHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryResponseDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FindAsync(request.Id)
            ?? throw new NotFoundException(ErrorMessageConstants.CategoryDoesNotExist);

        return category.ToOutDto();
    }
}
