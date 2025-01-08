namespace TaskManagementSystem.Constants;

public static class LoggingMessageConstants
{
    public const string TaskCreatedSuccessfully = "Task with Id {0} created successfully in Category with Id {1}.";
    public const string TaskUpdatedSuccessfully = "Task with Id {0} updated successfully.";
    public const string TaskUnlockedSuccessfully = "Task with Id {0} unlocked successfully.";
    public const string TaskRemovedSuccessfully = "Task with Id {0} removed successfully.";
    public const string TaskMovedSuccessfully = "Task with Id {0} moved to DeletedTasks successfully.";
    public const string TaskLockedSuccessfully = "Task with Id {0} locked successfully.";

    public const string CategoryCreatedSuccessfully = "Category with Id {0} created successfully.";
    public const string CategoryUpdatedSuccessfully = "Category with Id {0} updated successfully.";
    public const string CategoryDeletedSuccessfully = "Category with Id {0} deleted successfully.";

    public const string NotFoundException = "NotFoundException: {0}, Path: {1}";
    public const string InternalServerException = "InternalServerException: Path: {0}";
    public const string ConflictException = "ConflictException: {0}, Path: {1}";
    public const string BadRequestException = "BadRequestException: {0}, Path: {1}";
}