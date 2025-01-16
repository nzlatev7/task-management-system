using MediatR;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Features.Shared.DTOs;
using TaskManagementSystem.Features.Shared.Extensions;

namespace TaskManagementSystem.Features.Tasks;

public class GertTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskResponseDto>
{
    private readonly TaskManagementSystemDbContext _dbContext;

    public GertTaskByIdHandler(TaskManagementSystemDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskResponseDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var taskEntity = await _dbContext.Tasks.FindAsync(request.Id)
            ?? throw new NotFoundException(ErrorMessageConstants.TaskDoesNotExist);

        return taskEntity.ToOutDto();
    }
}
