namespace TaskManagementSystem.Logging;

public class LoggingDecorator<T> : ILogger<T>
{
    private readonly ILogger<T> _innerLogger;

    public LoggingDecorator(ILoggerFactory loggerFactory)
    {
        _innerLogger = loggerFactory.CreateLogger<T>();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _innerLogger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _innerLogger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _innerLogger.Log(logLevel, eventId, state, exception, formatter);
    }
}
