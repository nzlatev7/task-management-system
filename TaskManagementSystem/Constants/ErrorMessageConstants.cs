using TaskManagementSystem.Database.Models;

namespace TaskManagementSystem.Constants;

public static class ErrorMessageConstants
{
    public const string CategoryDoesNotExist = "Category does not exist";
    public const string TaskDoesNotExist = "Task does not exist";
    public const string AssociatedTasksToCategory = "There are associated tasks to this category";
    
    public const string TaskPriorityMustRepresentValidValues = $"The field {nameof(TaskEntity.Priority)} must have a value between 0 and 2 inclusive, representing 'Low', 'Medium', 'High'. You can also initialize it using these string values ('Low', 'Medium', 'High'). If no {nameof(TaskEntity.Priority)} is specified, the default value is 'Medium'.";
    public const string TaskStatusMustRepresentValidValues = $"The filed {nameof(TaskEntity.Status)} must be a value between 1 and 3 inclusive, representing 'InProgress', 'Completed', 'Archived'. You can also use these string values ('InProgress', 'Completed', 'Archived').";

    public const string OnlyCompletedTaskCanBeArchived = "Only Completed Task can be Archived";
    public const string ArchivedTaskCanNotBeMoved = "Archived Task can not be moved";
}