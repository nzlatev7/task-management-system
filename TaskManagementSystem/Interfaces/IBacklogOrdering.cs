using System.Collections.Generic;
using TaskManagementSystem.DTOs.Response;

namespace TaskManagementSystem.Interfaces;

public interface IBacklogOrdering
{
    IEnumerable<TaskResponseDto> Order(IEnumerable<TaskResponseDto> backlog);
}
