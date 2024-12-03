using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Helpers;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Tests.TestUtilities;

public static class TestResultBuilder
{
    public static List<TaskResponseDto> GetExpectedTasks(List<TaskEntity> tasks)
    {
        var result = new List<TaskResponseDto>();

        foreach (var task in tasks)
        {
            result.Add(task.ToOutDto());
        }

        return result;
    }

    public static List<CategoryResponseDto> GetExpectedCategories(List<Category> categories)
    {
        var result = new List<CategoryResponseDto>();

        foreach (var category in categories)
        {
            result.Add(category.ToOutDto());
        }

        return result;
    }
}
