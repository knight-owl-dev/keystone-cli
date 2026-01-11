using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.Application;

/// <summary>
/// The default implementation of <see cref="IConsole"/> that uses the system console.
/// </summary>
public class Console : IConsole
{
    /// <inheritdoc />
    public TextWriter Out => System.Console.Out;

    /// <inheritdoc />
    public TextWriter Error => System.Console.Error;
}
