using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Exceptions;

namespace TaskManagementSystem.ExceptionHandlers;

public class ConflictExceptionHanlder : IExceptionHandler       
{
    private readonly ILogger<ConflictExceptionHanlder> _logger;

    public ConflictExceptionHanlder(ILogger<ConflictExceptionHanlder> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ConflictException conflictException)
        {
            return false;
        }

        _logger.LogWarning(LoggingMessageConstants.ConflictException,
            conflictException.Message,
            httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Detail = conflictException.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
