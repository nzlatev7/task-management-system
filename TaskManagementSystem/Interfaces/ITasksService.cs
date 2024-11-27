using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface ITasksService
{
    Task<TaskResponseDto> CreateTaskAsync(TaskRequestDto taskDto);

    Task<List<TaskResponseDto>> GetAllTasksAsync();

    Task<TaskResponseDto?> GetTaskByIdAsync(int taskId);

    Task<TaskResponseDto?> UpdateTaskAsync(int taskId, TaskRequestDto taskDto);

    Task<bool> DeleteTaskAsync(int taskId);
}