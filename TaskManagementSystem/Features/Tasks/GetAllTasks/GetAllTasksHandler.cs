using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Tasks.GetAllTasks;

namespace TaskManagementSystem.Features.Tasks;

public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskResponseDto>>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public GetAllTasksHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<TaskResponseDto>> Handle(
        GetAllTasksQuery requestInstructions,
        CancellationToken cancellationToken)
    {
        var tasks = await _dbContext.Tasks
            .SortBy(requestInstructions)
            .Select(x => new TaskResponseDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                DueDate = x.DueDate,
                Priority = x.Priority,
                IsCompleted = x.IsCompleted,
                Status = x.Status,
                CategoryId = x.CategoryId
            }).ToListAsync();

        return tasks;
    }
}
