using System.Text;
using Microsoft.Extensions.Logging;


namespace Keystone.Cli.UnitTests.Logging;

/// <summary>
/// The captured log entry.
/// </summary>
/// <param name="LogLevel">The log level.</param>
/// <param name="EventId">Event ID.</param>
/// <param name="Message">Formatted message.</param>
/// <param name="Exception">The optional exception.</param>
public record LogEntry(LogLevel LogLevel, EventId EventId, string Message, Exception? Exception = null)
{
    /// <summary>
    /// Checks if the log entry matches the specified log level and contains the specified message fragment.
    /// </summary>
    /// <param name="level">The expected log level.</param>
    /// <param name="messageFragment">The expected message fragment.</param>
    /// <param name="comparison">The optional string comparer.</param>
    /// <returns>
    /// <c>true</c> if the log entry matches the specified log level and contains the specified message fragment;
    /// otherwise, <c>false</c>.
    /// </returns>
    public bool Is(LogLevel level, string messageFragment, StringComparison comparison = StringComparison.Ordinal)
        => this.LogLevel == level && this.Message.Contains(messageFragment, comparison);

    /// <summary>
    /// Returns a string representation of the log entry in the format:
    /// <c>[LogLevel]: Message (Exception message), event EventId</c>.
    /// </summary>
    /// <returns>
    /// A string representation of the log entry, including the log level, message, exception message (if any)
    /// and event ID (if any).
    /// </returns>
    public override string ToString()
    {
        var formatProvider = System.Globalization.CultureInfo.InvariantCulture;
        var builder = new StringBuilder();

        builder.Append(formatProvider, $"[{this.LogLevel}]: {this.Message}");

        if (this.Exception is not null)
        {
            builder.Append(formatProvider, $" ({this.Exception.Message})");
        }

        if (this.EventId != default)
        {
            builder.Append(formatProvider, $", event {this.EventId}");
        }

        return builder.ToString();
    }
}
