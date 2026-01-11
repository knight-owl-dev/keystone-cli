namespace Keystone.Cli.Application.Utility;

/// <summary>
/// Abstraction over the console for easier testing.
/// </summary>
public interface IConsole
{
    /// <summary>
    /// The standard output writer.
    /// </summary>
    TextWriter Out { get; }

    /// <summary>
    /// The standard error writer.
    /// </summary>
    TextWriter Error { get; }
}
