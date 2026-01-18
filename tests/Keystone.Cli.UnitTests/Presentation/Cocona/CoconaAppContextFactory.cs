using System.Reflection;
using Cocona;
using Cocona.Command;


namespace Keystone.Cli.UnitTests.Presentation.Cocona;

/// <summary>
/// Factory for creating <see cref="CoconaAppContext"/> instances in tests.
/// </summary>
public static class CoconaAppContextFactory
{
    /// <summary>
    /// Creates a <see cref="CoconaAppContext"/> with the specified cancellation token.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>A new <see cref="CoconaAppContext"/> instance.</returns>
    public static CoconaAppContext Create(CancellationToken cancellationToken = default)
    {
        var dummyMethod = typeof(CoconaAppContextFactory)
            .GetMethod(nameof(DummyCommand), BindingFlags.NonPublic | BindingFlags.Static)!;

        var commandDescriptor = new CommandDescriptor(
            dummyMethod,
            target: null,
            name: "dummy",
            aliases: [],
            description: string.Empty,
            metadata: [],
            parameters: [],
            options: [],
            arguments: [],
            overloads: [],
            optionLikeCommands: [],
            flags: CommandFlags.None,
            subCommands: null
        );

        return new CoconaAppContext(commandDescriptor, cancellationToken);
    }

    private static void DummyCommand()
    {
    }
}
