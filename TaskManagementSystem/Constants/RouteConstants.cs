namespace TaskManagementSystem.Constants;

public class RouteConstants
{
    public const string Api = "api";
    public const string IdParamName = "{id}";

    public const string Categories = $"{Api}/categories";
    public const string CategoryById = $"{Api}/categories/{IdParamName}";
    public const string TasksByCategory = $"{Api}/categories/{IdParamName}/tasks";
    public const string CompletionStatus = $"{Api}/categories/{IdParamName}/completion";

    public const string ReportForTasks = $"{Api}/reports/tasks";

    public const string Tasks = $"{Api}/tasks";
    public const string TaskById = $"{Api}/tasks/{IdParamName}";
}
