using TaskManagementSystem.Constants;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.LoggingDecorators;

public class TasksServiceLoggingDecorator : ITasksService
{
    private readonly ITasksService _wrappee;
    private readonly ILogger<ITasksService> _logger;

    public TasksServiceLoggingDecorator(
        ITasksService wrapee,
        ILogger<ITasksService> logger)
    {
        _wrappee = wrapee;
        _logger = logger;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto)
    {
        var task = await _wrappee.CreateTaskAsync(taskDto);

        _logger.LogInformation(LoggingMessageConstants.TaskCreatedSuccessfully, task.Id, task.CategoryId);

        return task;
    }

    public async Task<TaskResponseDto> CloneTaskAsync(int taskId, CloneTaskRequestDto? cloneDto)
    {
        var clonedTask = await _wrappee.CloneTaskAsync(taskId, cloneDto);

        _logger.LogInformation(LoggingMessageConstants.TaskClonedSuccessfully, clonedTask.Id, taskId);

        return clonedTask;
    }

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(GetAllTasksRequestDto sortBy)
    {
        return await _wrappee.GetAllTasksAsync(sortBy);
    }

    public async Task<IEnumerable<TaskResponseDto>> GetBacklogAsync(TaskKind kind)
    {
        return await _wrappee.GetBacklogAsync(kind);
    }

    public async Task<TaskResponseDto> GetTaskByIdAsync(int taskId)
    {
        return await _wrappee.GetTaskByIdAsync(taskId);
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto taskDto)
    {
        var task = await _wrappee.UpdateTaskAsync(taskId, taskDto);

        _logger.LogInformation(LoggingMessageConstants.TaskUpdatedSuccessfully, task.Id);

        return task;
    }

    public async Task UnlockTaskAsync(int taskId, UnlockTaskRequestDto unlockDto)
    {
        await _wrappee.UnlockTaskAsync(taskId, unlockDto);

        _logger.LogInformation(LoggingMessageConstants.TaskUnlockedSuccessfully, taskId);
    }

    public async Task<DeleteAction> DeleteTaskAsync(int taskId)
    {
        var deleteInstruction = await _wrappee.DeleteTaskAsync(taskId);

        LogProperMessageForDelete(deleteInstruction, taskId);

        return deleteInstruction;
    }

    private void LogProperMessageForDelete(DeleteAction instruction, int taskId)
    {
        switch (instruction)
        {
            case DeleteAction.Removed:
                _logger.LogInformation(LoggingMessageConstants.TaskRemovedSuccessfully, taskId);
                break;
            case DeleteAction.Moved:
                _logger.LogInformation(LoggingMessageConstants.TaskMovedSuccessfully, taskId);
                break;
            case DeleteAction.Locked:
                _logger.LogInformation(LoggingMessageConstants.TaskLockedSuccessfully, taskId);
                break;
            default:
                break;
        }
    }
}
