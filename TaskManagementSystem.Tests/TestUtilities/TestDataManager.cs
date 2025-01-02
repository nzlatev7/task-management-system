using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Database;
using TaskManagementSystem.Enums;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Tests.TestUtilities;

public class TestDataManager
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public TestDataManager(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskEntity> InsertTaskAsync(string title, DateTime dueDate, Priority priority, Status status, int categoryId)
    {
        var task = new TaskEntity()
        {
            Title = title,
            DueDate = dueDate,
            Priority = priority,
            Status = status,
            CategoryId = categoryId
        };

        await _dbContext.AddAsync(task);
        await _dbContext.SaveChangesAsync();

        return task;
    }

    public async Task<List<TaskEntity>> InsertTasksAsync(
        int count,
        int categoryId,
        Priority tasksPriority = Priority.Medium,
        Status tasksStatus = Status.Pending)
    {
        var tasks = new List<TaskEntity>();

        var dateTime = DateTime.UtcNow;
        var roundedDateTime = new DateTime(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond), DateTimeKind.Utc);

        for (int i = 0; i < count; i++)
        {
            var task = new TaskEntity()
            {
                Title = $"task{i}",
                Description = nameof(TaskEntity.Description),
                DueDate = roundedDateTime.AddDays(i),
                Priority = tasksPriority,
                IsCompleted = false,
                Status = tasksStatus,
                CategoryId = categoryId
            };

            tasks.Add(task);
        }

        await _dbContext.AddRangeAsync(tasks);
        await _dbContext.SaveChangesAsync();

        return tasks;
    }

    public async Task<int> InsertCategoryAsync()
    {
        var category = new CategoryEntity()
        {
            Name = $"category",
            Description = nameof(CategoryEntity.Description)
        };

        await _dbContext.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return category.Id;
    }

    public async Task<List<CategoryEntity>> InsertCategoriesAsync(int count)
    {
        var categories = new List<CategoryEntity>();

        for (int i = 0; i < count; i++)
        {
            var category = new CategoryEntity()
            {
                Name = $"category{i}",
                Description = nameof(CategoryEntity.Description)
            };

            categories.Add(category);
        }

        await _dbContext.AddRangeAsync(categories);
        await _dbContext.SaveChangesAsync();

        return categories;
    }

    public async Task ClearTestDataAsync()
    {
        await _dbContext.Tasks.ExecuteDeleteAsync();
        await _dbContext.Categories.ExecuteDeleteAsync();
    }
}
