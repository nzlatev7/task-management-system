using TaskManagementSystem.Database.Models;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Extensions;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Categories.Shared;
using TaskManagementSystem.Features.Reports.DTOs;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Tasks;

namespace TaskManagementSystem.Tests.TestUtilities;

public static class TestResultBuilder
{
    public static TaskResponseDto GetExpectedTask(TaskEntity task)
        => CreateBaseTaskResponse(task.Id, task.Title, task.Description, task.DueDate, task.Priority, task.IsCompleted, task.Status, task.CategoryId);

    public static TaskResponseDto GetExpectedTask(int taskId, CreateTaskCommand task)
        => CreateBaseTaskResponse(taskId, task.Title, task.Description, task.DueDate, task.Priority, isCompleted: false, Status.Pending, task.CategoryId);

    public static TaskResponseDto GetExpectedTask(int taskId, UpdateTaskCommand task)
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

    public static List<TaskResponseDto> GetOrderedTasks(List<TaskResponseDto> tasks, GetAllTasksQuery sortBy)
    {
        switch (sortBy.Property)
        {
            case SortingTaskProperty.Id:
                tasks = tasks.OrderBy(x => x.Id, sortBy.IsAscending);
                break;
            case SortingTaskProperty.Title:
                tasks = tasks.OrderBy(x => x.Title, sortBy.IsAscending);
                break;
            case SortingTaskProperty.DueDate:
                tasks = tasks.OrderBy(x => x.DueDate, sortBy.IsAscending);
                break;
            case SortingTaskProperty.Priority:
                tasks = tasks.OrderBy(x => x.Priority, sortBy.IsAscending);
                break;
            case SortingTaskProperty.Status:
                tasks = tasks.OrderBy(x => x.Status, sortBy.IsAscending);
                break;
            case SortingTaskProperty.CategoryId:
                tasks = tasks.OrderBy(x => x.CategoryId, sortBy.IsAscending);
                break;
            default:
                break;
        }

        return tasks;
    }

    private static List<TaskResponseDto> OrderBy<T>(
        this List<TaskResponseDto> tasks,
        Func<TaskResponseDto, T> expression,
        bool isAscending)
    {
        return isAscending 
            ? tasks.OrderBy(expression).ToList() 
            : tasks.OrderByDescending(expression).ToList();
    }

    public static CategoryResponseDto GetExpectedCategory(int categoryId, CreateCategoryCommand request)
        => CreateBaseCategoryResponse(categoryId, request.Name, request.Description);

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

    public static DeletedTaskEntity GetExpectedDeletedTask(TaskEntity task)
    {
        return new DeletedTaskEntity()
        {
            TaskId = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority,
            Status = task.Status,
            CategoryId = task.CategoryId
        };
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
