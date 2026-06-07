using Microsoft.Extensions.Logging;

namespace Thushar.Logging;

public class LoggerOption
{
    internal bool StructuredLogging { get; private set; } = false;
    internal LogLevel MinimumLogLevel { get; private set; } = LogLevel.Information;

    public LoggerOption UseStructuredLogging()
    {
        StructuredLogging = true;
        return this;
    }

    public LoggerOption UseNormalLogging()
    {
        StructuredLogging = false;
        return this;
    }
    
    public LoggerOption SetMinimumLogLevel(
        LogLevel logLevel)
    {
        MinimumLogLevel = logLevel;
        return this;
    }
}