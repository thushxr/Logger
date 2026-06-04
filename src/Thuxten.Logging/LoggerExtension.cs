using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

        if (options.MaskingRules.Count > 0)
        {
            var processor = new MaskingProcessor(options.MaskingRules);
            services.AddSingleton(typeof(ILogger<>), typeof(MaskingLogger<>));
            services.AddSingleton(processor);
        }
        else
        {
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        }

        return services;
    }
}