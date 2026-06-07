using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Thushar.Logging;

public static class LoggerExtension
{
    public static IServiceCollection AddLogger(
        this IServiceCollection services,
        Action<LoggerOption>? configure = null)
    {
        var options = new LoggerOption();

        configure?.Invoke(options);

        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            if (options.StructuredLogging)
            {
                builder.AddJsonConsole(jsonOptions =>
                {
                    jsonOptions.IncludeScopes = true;
                    jsonOptions.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    jsonOptions.UseUtcTimestamp = true;
                    jsonOptions.JsonWriterOptions = new JsonWriterOptions
                    {
                        Indented = true
                    };
                });
            }
            else
            {
                builder.AddSimpleConsole(consoleOptions =>
                {
                    consoleOptions.IncludeScopes = true;
                    consoleOptions.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    consoleOptions.UseUtcTimestamp = true;
                });
            }

            builder.SetMinimumLevel(options.MinimumLogLevel);
        });

        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        return services;
    }
}