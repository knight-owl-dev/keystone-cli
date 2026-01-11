using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.UnitTests.Application.Utility;

/// <summary>
/// The null implementation of <see cref="IConsole"/> that discards all output.
/// </summary>
/// <remarks>
/// Use for testing purposes.
/// </remarks>
public class NullConsole : IConsole
{
    /// <summary>
    /// The only instance of <see cref="NullConsole"/>.
    /// </summary>
    public static NullConsole Instance { get; } = new();

    /// <summary>
    /// The private constructor to prevent external instantiation.
    /// </summary>
    private NullConsole()
    {
    }

    /// <inheritdoc />
    public TextWriter Out => TextWriter.Null;

    /// <inheritdoc />
    public TextWriter Error => TextWriter.Null;
}
