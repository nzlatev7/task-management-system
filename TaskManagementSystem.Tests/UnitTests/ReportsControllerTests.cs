using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagementSystem.Controllers;
using TaskManagementSystem.DTOs.Request;
using TaskManagementSystem.DTOs.Response;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Interfaces;

namespace TaskManagementSystem.Tests.UnitTests;

public sealed class ReportsControllerTests
{
    private readonly Mock<IReportsService> _reportsServiceMock;
    private readonly ReportsControllers _reportsControllers;

    public ReportsControllerTests()
    {
        _reportsServiceMock = new Mock<IReportsService>();
        _reportsControllers = new ReportsControllers(_reportsServiceMock.Object);
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

        _reportsServiceMock.Setup(service => service.GetReportForTasksAsync(requestDto)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _reportsControllers.GetReportForTasks(requestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, okResult.Value);
        _reportsServiceMock.Verify(services => services.GetReportForTasksAsync(requestDto), Times.Once);
    }

    #endregion
}
