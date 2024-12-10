using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface ITasksService
{
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto);

    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(bool sortByPriorityAscending);

    Task<TaskResponseDto> GetTaskByIdAsync(int taskId);

    Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto taskDto);

    Task DeleteTaskAsync(int taskId);
}