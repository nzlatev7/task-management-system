﻿using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface ITasksService
{
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto taskDto);

    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(GetAllTasksRequestDto sortBy);

    Task<TaskResponseDto> GetTaskByIdAsync(int taskId);

    Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskRequestDto taskDto);

    Task UnlockTaskAsync(int taskId, UnlockTaskRequestDto unlockDto);

    Task DeleteTaskAsync(int taskId);
}