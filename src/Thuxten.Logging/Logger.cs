using Microsoft.Extensions.Logging;
using MsLogger = Microsoft.Extensions.Logging;

namespace Thuxten.Logging;

public sealed class Logger<T> : ILogger<T> where T : class
{
    private readonly MsLogger.ILogger<T> _logger;

    public Logger(
        MsLogger.ILogger<T> logger) 
    {
        _logger = logger;
    }

    public IDisposable BeginTraceScope(
        string traceId)
    {
        return _logger?.BeginScope("TraceId: {TraceId}", traceId);
    }

    public void LogInformation(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Information, exception, message, args);
    }

    public void LogInformation(
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Information, message, args);
    }

    public void LogError(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Error, exception, message, args);
    }

    public void LogError(
       string message,
       params object[] args)
    {
        _logger.Log(LogLevel.Error, message, args);
    }

    public void LogWarning(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Warning, exception, message, args);
    }

    public void LogWarning(
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Warning, message, args);
    }

    public void LogCritical(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Critical, exception, message, args);
    }

    public void LogCritical(
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Critical, message, args);
    }

    public void LogDebug(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Debug, exception, message, args);
    }

    public void LogDebug(
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Debug, message, args);
    }

    public void LogTrace(
        Exception exception,
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Trace, exception, message, args);
    }

    public void LogTrace(
        string message,
        params object[] args)
    {
        _logger.Log(LogLevel.Trace, message, args);
    }
}