using Microsoft.Extensions.Logging;
using MsLogger = Microsoft.Extensions.Logging;

namespace Thuxten.Logging;

public class Logger<T> : ILogger<T> where T : class
{
    private readonly MsLogger.ILogger<T> _logger;

    public Logger(MsLogger.ILogger<T> logger) 
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogError(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    public IDisposable BeginTraceScope(string traceId)
    {
        return _logger.BeginScope("TraceId: {TraceId}", traceId);
    }
}