using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Checkers;

public class CategoryChecker : ICategoryChecker
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public CategoryChecker(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId);
    }
}
