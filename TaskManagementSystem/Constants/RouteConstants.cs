namespace TaskManagementSystem.Constants;

public class RouteConstants
{
    public const string Api = "api";
    public const string Id = "id";

    public const string Categories = $"{Api}/categories";
    public const string CategoryById = $"{Api}/categories/{Id}";
    public const string TasksByCategory = $"{Api}/categories/{Id}/tasks";
    public const string CompletionStatus = $"{Api}/categories/{Id}/completion";

    public const string ReportForTasks = $"{Api}/reports/tasks";

    public const string Tasks = $"{Api}/tasks";
    public const string TaskById = $"{Api}/tasks/{Id}";
}
