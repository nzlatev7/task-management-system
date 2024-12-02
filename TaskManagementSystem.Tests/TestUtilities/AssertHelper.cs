using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Tests.TestUtilities;

public static class AssertHelper
{
    public static void Equal(TaskRequestDto expectedTask, TaskEntity actualTask)
    {
        Assert.Equal(expectedTask.Title, actualTask.Title);
        Assert.Equal(expectedTask.Description, actualTask.Description);
        Assert.Equal(expectedTask.DueDate, actualTask.DueDate);
        Assert.Equal(expectedTask.IsCompleted, actualTask.IsCompleted);
        Assert.Equal(expectedTask.CategoryId, actualTask.CategoryId);
    }

    public static void Equal(TaskRequestDto expectedTask, TaskResponseDto actualTask)
    {
        Assert.Equal(expectedTask.Title, actualTask.Title);
        Assert.Equal(expectedTask.Description, actualTask.Description);
        Assert.Equal(expectedTask.DueDate, actualTask.DueDate);
        Assert.Equal(expectedTask.IsCompleted, actualTask.IsCompleted);
        Assert.Equal(expectedTask.CategoryId, actualTask.CategoryId);
    }

    public static void Equal(TaskEntity expectedTask, TaskResponseDto actualTask)
    {
        Assert.Equal(expectedTask.Title, actualTask.Title);
        Assert.Equal(expectedTask.Description, actualTask.Description);
        Assert.Equal(expectedTask.DueDate, actualTask.DueDate);
        Assert.Equal(expectedTask.IsCompleted, actualTask.IsCompleted);
        Assert.Equal(expectedTask.CategoryId, actualTask.CategoryId);
    }

    public static void Equal(TaskEntity expectedTask, TaskEntity actualTask)
    {
        Assert.Equal(expectedTask.Title, actualTask.Title);
        Assert.Equal(expectedTask.Description, actualTask.Description);
        Assert.Equal(expectedTask.DueDate, actualTask.DueDate);
        Assert.Equal(expectedTask.IsCompleted, actualTask.IsCompleted);
        Assert.Equal(expectedTask.CategoryId, actualTask.CategoryId);
    }

    public static void Equal(CategoryRequestDto expectedCategory, CategoryResponseDto actualCategory)
    {
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
        Assert.Equal(expectedCategory.Description, actualCategory.Description);
    }

    public static void Equal(CategoryRequestDto expectedCategory, Category actualCategory)
    {
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
        Assert.Equal(expectedCategory.Description, actualCategory.Description);
    }

    public static void Equal(Category expectedCategory, CategoryResponseDto actualCategory)
    {
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
        Assert.Equal(expectedCategory.Description, actualCategory.Description);
    }

    public static void Equal(Category expectedCategory, Category actualCategory)
    {
        Assert.Equal(expectedCategory.Name, actualCategory.Name);
        Assert.Equal(expectedCategory.Description, actualCategory.Description);
    }
}