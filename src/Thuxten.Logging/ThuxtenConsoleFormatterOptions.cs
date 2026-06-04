using Microsoft.Extensions.Logging.Console;

namespace Thuxten.Logging;

/// <summary>
/// Options for <see cref="ThuxtenConsoleFormatter"/>.
/// </summary>
public sealed class ThuxtenConsoleFormatterOptions : ConsoleFormatterOptions
{
    /// <summary>
    /// When <see langword="true"/> the entry is written as colored, indented JSON
    /// instead of a single colored text line.
    /// </summary>
    public bool Structured { get; set; }
}
