using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Helpers;

namespace TaskManagementSystem.Tests.TestUtilities;

public static class TestResultBuilder
{
    public static TaskResponseDto GetExpectedTask(TaskEntity task)
        => CreateBaseTaskResponse(task.Id, task.Title, task.Description, task.DueDate, task.Priority, task.IsCompleted, task.Status, task.CategoryId);

    public static TaskResponseDto GetExpectedTask(int taskId, CreateTaskRequestDto task)
        => CreateBaseTaskResponse(taskId, task.Title, task.Description, task.DueDate, task.Priority, isCompleted: false, Status.Pending, task.CategoryId);

    public static TaskResponseDto GetExpectedTask(int taskId, UpdateTaskRequestDto task)
        => CreateBaseTaskResponse(taskId, task.Title, task.Description, task.DueDate, task.Priority, isCompleted: false, task.Status, task.CategoryId);

    public static List<TaskResponseDto> GetExpectedTasks(List<TaskEntity> tasks)
    {
        var result = new List<TaskResponseDto>();

        foreach (var task in tasks)
        {
            var taskResult = GetExpectedTask(task);
            result.Add(taskResult);
        }

        return result;
    }

    public static List<TaskResponseDto> GetOrderedTasks(List<TaskResponseDto> tasks, bool sortByPriorityAscending)
    {
        if (sortByPriorityAscending)
            tasks = tasks.OrderBy(x => x.Priority).ToList();
        else
            tasks = tasks.OrderByDescending(x => x.Priority).ToList();

        return tasks;
    }

    public static CategoryResponseDto GetExpectedCategory(int categoryId, CategoryRequestDto category)
        => CreateBaseCategoryResponse(categoryId, category.Name, category.Description);

    public static CategoryResponseDto GetExpectedCategory(CategoryEntity category)
        => CreateBaseCategoryResponse(category.Id, category.Name, category.Description);

    public static List<CategoryResponseDto> GetExpectedCategories(List<CategoryEntity> categories)
    {
        var result = new List<CategoryResponseDto>();

        foreach (var category in categories)
        {
            var categoryResult = GetExpectedCategory(category);
            result.Add(categoryResult);
        }

        return result;
    }

    public static List<ReportTasksResponseDto> GetExpectedReport(List<CategoryEntity> categories, List<TaskEntity> tasks)
    {
        var reportResult = new List<ReportTasksResponseDto>();

        foreach (var category in categories)
        {
            var tasksForCategory = tasks
                .Where(x => x.CategoryId == category.Id)
                .Select(x => x.ToOutDto())
                .ToList();

            var tasksResult = new ReportTasksResponseDto()
            {
                CategoryId = category.Id,
                Tasks = tasksForCategory
            };

            reportResult.Add(tasksResult);
        }

        return reportResult;
    }

    private static TaskResponseDto CreateBaseTaskResponse(
        int id,
        string title,
        string? description,
        DateTime dueDate,
        Priority? priority,
        bool isCompleted,
        Status status,
        int categoryId)
    {
        return new TaskResponseDto()
        {
            Id = id,
            Title = title,
            Description = description,
            DueDate = dueDate,
            Priority = priority ?? Priority.Medium,
            IsCompleted = isCompleted,
            Status = status,
            CategoryId = categoryId
        };
    }

    private static CategoryResponseDto CreateBaseCategoryResponse(
        int id,
        string name,
        string? description)
    {
        return new CategoryResponseDto()
        {
            Id = id,
            Name = name,
            Description = description,
        };
    }
}
