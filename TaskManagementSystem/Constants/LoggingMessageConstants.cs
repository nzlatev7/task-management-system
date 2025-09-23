namespace TaskManagementSystem.Constants;

public static class LoggingMessageConstants
{
    public const string TaskCreatedSuccessfully = "Task with Id {0} created successfully in CategoryId {1}.";
    public const string TaskClonedSuccessfully = "Task with Id {0} cloned successfully from task {1}.";
    public const string TaskUpdatedSuccessfully = "Task with Id {0} updated successfully.";
    public const string TaskUnlockedSuccessfully = "Task with Id {0} unlocked successfully.";
    public const string TaskRemovedSuccessfully = "Task with Id {0} removed successfully.";
    public const string TaskMovedSuccessfully = "Task with Id {0} moved to DeletedTasks successfully.";
    public const string TaskLockedSuccessfully = "Task with Id {0} locked successfully.";

    public const string CategoryCreatedSuccessfully = "Category with Id {0} created successfully.";
    public const string CategoryUpdatedSuccessfully = "Category with Id {0} updated successfully.";
    public const string CategoryDeletedSuccessfully = "Category with Id {0} deleted successfully.";
}
