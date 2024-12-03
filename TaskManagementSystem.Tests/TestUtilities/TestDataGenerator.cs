using TaskManagementSystem.Models;

namespace TaskManagementSystem.Tests.TestUtilities;

public class TestDataGenerator
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public TestDataGenerator(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<TaskEntity>> InsertTasks(int count, int categoryId)
    {
        var tasks = new List<TaskEntity>();
        var dateTime = DateTime.UtcNow;

        for (int i = 1; i <= count; i++)
        {
            var task = new TaskEntity()
            {
                Title = $"task{i}",
                Description = nameof(TaskEntity.Description),
                DueDate = dateTime.AddDays(i),
                IsCompleted = false,
                CategoryId = categoryId
            };

            tasks.Add(task);
        }

        await _dbContext.AddRangeAsync(tasks);
        await _dbContext.SaveChangesAsync();

        return tasks;
    }

    public async Task<List<Category>> InsertCategories(int count)
    {
        var categories = new List<Category>();

        for (int i = 1; i <= count; i++)
        {
            var category = new Category()
            {
                Name = $"category{i}",
                Description = nameof(TaskEntity.Description)
            };

            categories.Add(category);
        }

        await _dbContext.AddRangeAsync(categories);
        await _dbContext.SaveChangesAsync();

        return categories;
    }
}
