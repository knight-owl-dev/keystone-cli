using Keystone.Cli.Application.Utility;


namespace Keystone.Cli.Application;

/// <summary>
/// The default implementation of <see cref="IConsole"/> that uses the system console.
/// </summary>
public class SystemConsole : IConsole
{
    /// <inheritdoc />
    public TextWriter Out => Console.Out;

    /// <inheritdoc />
    public TextWriter Error => Console.Error;
}
