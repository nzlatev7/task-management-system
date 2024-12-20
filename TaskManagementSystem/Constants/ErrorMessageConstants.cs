using TaskManagementSystem.Database.Models;
using TaskManagementSystem.DTOs.Request;

namespace TaskManagementSystem.Constants;

public static class ErrorMessageConstants
{
    public const string DateInUtc = "The date must be in UTC.";
    public const string DateInFuture = "The date must be in the future";
    public const string DueBeforeEarlierThanDueAfter = "DueBefore cannot be earlier than the DueAfter.";

    public const string CategoryDoesNotExist = "Category does not exist";
    public const string TaskDoesNotExist = "Task does not exist";
    public const string LockedTaskWithIdDoesNotExist = "Locked Task with this id does not exist";
    public const string AssociatedTasksToCategory = "There are associated tasks to this category. In order to delete the Category, you need to remove all tasks that are connected to it.";
    public const string CategoryWithoutTasks = "In order to get the Completion Status, you need to have at least one Task which is not Archived";
    
    public const string TaskPriorityMustRepresentValidValues = $"The field {nameof(TaskEntity.Priority)} must have a value between 0 and 2 inclusive, representing 'Low', 'Medium', 'High'. You can also initialize it using these string values ('Low', 'Medium', 'High'). If no {nameof(TaskEntity.Priority)} is specified, the default value is 'Medium'.";
    public const string UnlockTaskStatusMustRepresentValidValues = $"The filed {nameof(TaskEntity.Status)} must be a value between 1 and 3 inclusive, representing 'InProgress', 'Completed', 'Archived'. You can also use these string values ('InProgress', 'Completed', 'Archived').";
    public const string UpdateTaskStatusMustRepresentValidValues = $"The filed {nameof(TaskEntity.Status)} must be a value between 0 and 2 inclusive, representing 'Pending', 'InProgress', 'Completed'. You can also use these string values ('Pending', 'InProgress', 'Completed').";
    public const string SortingTaskPropertyMustRepresentValidValues = $"The filed {nameof(GetAllTasksRequestDto.Property)} must have a value between 0 and 5 inclusive, reresenting 'Id', 'Title', 'DueDate', 'Priority', 'Status', 'CategoryId'. You can also initialize it using these string values ('Id', 'Title', 'DueDate', 'Priority', 'Status', 'CategoryId').";

    public const string OnlyCompletedTaskCanBeArchived = "Only Completed Task can be Archived";
    public const string ArchivedTaskCanNotBeEdited = "Archived Task can not be edited";
    public const string ArchivedTaskCanNotBeDeleted = "Archived Task can not be deleted";
    public const string LockedTaskCanNotBeEdited = "Locked Task can not be edited";
    public const string TaskAlreadyLocked = "Task is already Locked";

}