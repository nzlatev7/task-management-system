using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public CategoryRepository(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId);
    }
}
