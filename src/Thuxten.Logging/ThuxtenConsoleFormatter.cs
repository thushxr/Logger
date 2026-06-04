using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Thuxten.Logging;

/// <summary>
/// A custom console formatter that writes each log entry using a color and an
/// icon based on its <see cref="LogLevel"/>. Supports both a single-line text
/// mode and a colored, structured (JSON) mode.
/// </summary>
public sealed class ThuxtenConsoleFormatter : ConsoleFormatter, IDisposable
{
    public const string FormatterName = "thuxten";

    // ANSI escape codes.
    private const string Reset = "\u001b[0m";

    private readonly IDisposable? _optionsReloadToken;
    private ThuxtenConsoleFormatterOptions _options;

    public ThuxtenConsoleFormatter(IOptionsMonitor<ThuxtenConsoleFormatterOptions> options)
        : base(FormatterName)
    {
        _options = options.CurrentValue;
        _optionsReloadToken = options.OnChange(updated => _options = updated);
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (message is null && logEntry.Exception is null)
        {
            return;
        }

        var (color, icon, label) = GetStyle(logEntry.LogLevel);

        if (_options.Structured)
        {
            WriteStructured(logEntry, message, color, icon, label, scopeProvider, textWriter);
            return;
        }

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Example line:  [2024-01-01 10:00:00] ✔ info: Logging.Example.Program[0]: This is an information log.
        textWriter.Write(color);
        textWriter.Write($"[{timestamp}] {icon} {label}: {logEntry.Category}[{logEntry.EventId.Id}]: {message}");
        textWriter.Write(Reset);
        textWriter.Write(Environment.NewLine);

        if (logEntry.Exception is not null)
        {
            textWriter.Write(color);
            textWriter.Write(logEntry.Exception.ToString());
            textWriter.Write(Reset);
            textWriter.Write(Environment.NewLine);
        }
    }

    private static void WriteStructured<TState>(
        in LogEntry<TState> logEntry,
        string? message,
        string color,
        string icon,
        string label,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
        {
            Indented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }))
        {
            writer.WriteStartObject();
            writer.WriteString("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            writer.WriteString("Level", label);
            writer.WriteString("Icon", icon);
            writer.WriteNumber("EventId", logEntry.EventId.Id);
            writer.WriteString("Category", logEntry.Category);
            writer.WriteString("Message", message);

            // Write the structured state (the named message-template arguments,
            // e.g. {data}) as a nested "State" object.
            if (logEntry.State is IReadOnlyList<KeyValuePair<string, object?>> stateValues
                && stateValues.Count > 0)
            {
                writer.WriteStartObject("State");
                foreach (var item in stateValues)
                {
                    // {OriginalFormat} is the raw template; skip it to avoid noise.
                    if (item.Key == "{OriginalFormat}")
                    {
                        continue;
                    }

                    WriteValue(writer, item.Key, item.Value);
                }

                writer.WriteEndObject();
            }

            WriteScopes(writer, scopeProvider);

            if (logEntry.Exception is not null)
            {
                writer.WriteString("Exception", logEntry.Exception.ToString());
            }

            writer.WriteEndObject();
        }

        var json = System.Text.Encoding.UTF8.GetString(stream.ToArray());

        // Apply the level color to the whole JSON block.
        textWriter.Write(color);
        textWriter.Write(json);
        textWriter.Write(Reset);
        textWriter.Write(Environment.NewLine);
    }

    private static void WriteScopes(Utf8JsonWriter writer, IExternalScopeProvider? scopeProvider)
    {
        if (scopeProvider is null)
        {
            return;
        }

        var hasScopes = false;
        scopeProvider.ForEachScope((scope, state) =>
        {
            if (!hasScopes)
            {
                state.WriteStartArray("Scopes");
                hasScopes = true;
            }

            if (scope is IReadOnlyList<KeyValuePair<string, object?>> scopeValues)
            {
                state.WriteStartObject();
                foreach (var item in scopeValues)
                {
                    if (item.Key == "{OriginalFormat}")
                    {
                        continue;
                    }

                    WriteValue(state, item.Key, item.Value);
                }

                state.WriteEndObject();
            }
            else
            {
                state.WriteStringValue(scope?.ToString());
            }
        }, writer);

        if (hasScopes)
        {
            writer.WriteEndArray();
        }
    }

    private static void WriteValue(Utf8JsonWriter writer, string key, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNull(key);
                break;
            case bool b:
                writer.WriteBoolean(key, b);
                break;
            case int i:
                writer.WriteNumber(key, i);
                break;
            case long l:
                writer.WriteNumber(key, l);
                break;
            case double d:
                writer.WriteNumber(key, d);
                break;
            case decimal m:
                writer.WriteNumber(key, m);
                break;
            default:
                writer.WriteString(key, value.ToString());
                break;
        }
    }


    private static (string Color, string Icon, string Label) GetStyle(LogLevel level) => level switch
    {
        // \u001b[<code>m  ->  90=gray 32=green 36=cyan 33=yellow 31=red 97=white 41=red background
        LogLevel.Trace => ("\u001b[90m", "🔍", "trce"),
        LogLevel.Debug => ("\u001b[36m", "🐞", "dbug"),
        LogLevel.Information => ("\u001b[32m", "✅", "info"),
        LogLevel.Warning => ("\u001b[33m", "⚠️", "warn"),
        LogLevel.Error => ("\u001b[31m", "❌", "fail"),
        LogLevel.Critical => ("\u001b[97;41m", "🔥", "crit"),
        _ => ("\u001b[0m", " ", "info"),
    };

    public void Dispose() => _optionsReloadToken?.Dispose();
}
