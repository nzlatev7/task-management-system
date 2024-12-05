using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Constants;

public static class ErrorMessageConstants
{
    public const string CategoryDoesNotExist = "Category does not exist";
    public const string TaskDoesNotExist = "Task does not exist";
    public const string AssociatedTasksToCategory = "There are associated tasks to this category";
    public const string TaskPriority = $"The field {nameof(TaskEntity.Priority)} must have a value between 0 and 2, representing 'Low', 'Medium', 'High'. You can also initialize it using these string values ('Low', 'Medium', 'High'). If no {nameof(TaskEntity.Priority)} is specified, the default value is 'Medium'.";
}