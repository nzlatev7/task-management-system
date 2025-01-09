using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Features.Reports;
using TaskManagementSystem.Features.Reports.DTOs;
using TaskManagementSystem.Features.Shared.DTOs;

namespace TaskManagementSystem.Tests.UnitTests.Controllers;

public sealed class ReportsControllerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly ReportsControllers _reportsControllers;

    public ReportsControllerTests()
    {
        _mediator = new Mock<IMediator>();
        _reportsControllers = new ReportsControllers(_mediator.Object);
    }

    #region GetReportForTasks

    [Fact]
    public async Task GetReportForTasks_ReturnsOkResultWithTasksGroupedByCategory()
    {
        // Arrange
        var requestDto = new ReportTasksRequestDto()
        {
            Status = Status.Pending,
            Priority = Priority.Low,
            DueBefore = DateTime.UtcNow.AddDays(-3),
            DueAfter = DateTime.UtcNow.AddDays(3),
        };

        var expectedResponse = new List<ReportTasksResponseDto>()
        {
            new ReportTasksResponseDto()
            {
                CategoryId = 1,
                Tasks = new List<TaskResponseDto>()
                {
                    new TaskResponseDto()
                    {
                        Id = 1,
                        Title = "1"
                    },
                    new TaskResponseDto()
                    {
                        Id = 2,
                        Title = "2"
                    }
                }
            }
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetReportForTasksQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _reportsControllers.GetReportForTasks(requestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);

        _mediator.Verify(m => m.Send(It.IsAny<GetReportForTasksQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
