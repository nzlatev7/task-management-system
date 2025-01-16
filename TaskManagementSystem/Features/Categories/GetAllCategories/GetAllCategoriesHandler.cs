using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Features.Categories.Shared;

namespace TaskManagementSystem.Features.Categories;

public class GetAllCategoriesHandler 
    : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryResponseDto>>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public GetAllCategoriesHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CategoryResponseDto>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _dbContext.Categories
            .Select(x => new CategoryResponseDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            }).ToListAsync();

        return categories;
    }
}