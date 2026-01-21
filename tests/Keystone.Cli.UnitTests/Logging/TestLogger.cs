using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace Keystone.Cli.UnitTests.Logging;

/// <summary>
/// The test logger facilitates integration with Microsoft <see cref="ILogger{TCategoryName}"/>
/// structured logging.
/// </summary>
/// <remarks>
/// Microsoft logger uses extension methods and internal types to format their messages, which makes
/// testing with mocking frameworks impossible. Use this wrapper to capture log messages to verify
/// the <i>contents</i> of <see cref="CapturedLogEntries"/> in your tests.
/// </remarks>
/// <param name="minimumLogLevel">Limit the minimum log level to capture to reduce the noise level in tests.</param>
/// <typeparam name="TCategoryName">The type whose name is used for the logger category name.</typeparam>
public class TestLogger<TCategoryName>(LogLevel minimumLogLevel = LogLevel.Trace)
    : ILogger<TCategoryName>
{
    /// <summary>
    /// All captured log entries.
    /// </summary>
    public Collection<LogEntry> CapturedLogEntries { get; } = [];

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
        => Substitute.For<IDisposable>();

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
        => logLevel >= minimumLogLevel;

    /// <inheritdoc />
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        ArgumentNullException.ThrowIfNull(formatter);

        var entry = new LogEntry(
            logLevel,
            eventId,
            formatter(state, exception),
            exception
        );

        this.CapturedLogEntries.Add(entry);
    }
}
