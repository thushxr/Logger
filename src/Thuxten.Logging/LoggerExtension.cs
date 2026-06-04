using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Text.Json;

namespace Thuxten.Logging;

public static class LoggerExtension
{
    public static IServiceCollection AddThuxtenLogging(
        this IServiceCollection services,
        Action<LoggerOption>? configure = null)
    {
        var options = new LoggerOption();

        configure?.Invoke(options);

        services.AddLogging(builder =>
        {
            builder.ClearProviders();

            if (options.ColoredLogging)
            {
                // Ensure the console can render the Unicode icons used by the
                // formatter; otherwise they show up as '?'.
                try
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                }
                catch
                {
                    // Some redirected/non-interactive streams don't allow setting
                    // the encoding; safe to ignore.
                }

                var structured = options.StructuredLogging;

                builder.AddConsole(consoleOptions =>
                {
                    consoleOptions.FormatterName = ThuxtenConsoleFormatter.FormatterName;
                });
                builder.AddConsoleFormatter<ThuxtenConsoleFormatter, ThuxtenConsoleFormatterOptions>(
                    formatterOptions => formatterOptions.Structured = structured);
            }
            else if (options.StructuredLogging)
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