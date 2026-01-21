using System.Reflection;
using Cocona;
using Cocona.Application;
using Keystone.Cli.Presentation;


namespace Keystone.Cli.UnitTests.Presentation.Cocona;

[TestFixture, Parallelizable(ParallelScope.All)]
public class CoconaCommandMethodConventionsTests
{
    private const string PresentationNamespace = "Keystone.Cli.Presentation";

    /// <summary>
    /// Cocona exposes method parameters as CLI options by default. <see cref="CancellationToken"/> parameters
    /// should not appear in help output as they are framework infrastructure, not user-actionable arguments.
    /// </summary>
    /// <remarks>
    /// To access <see cref="CancellationToken"/> in a command method, inject <see cref="ICoconaAppContextAccessor"/>
    /// via the constructor and use its <see cref="ICoconaAppContextAccessor.Current"/> property,
    /// e.g., <c>contextAccessor.Current?.CancellationToken</c>.
    /// </remarks>
    [Test]
    public void CommandMethods_ShouldNotExposeCancellationTokenAsParameter()
    {
        MethodInfo[] commandMethods = [..DiscoverCommandMethods().Distinct()];

        string[] violations =
        [
            ..commandMethods.SelectMany(method => method
                .GetParameters()
                .Where(p => p.ParameterType == typeof(CancellationToken))
                .Select(p => $"{method.DeclaringType!.Name}.{method.Name}({p.Name})")
            ),
        ];

        Assert.That(
            violations,
            Is.Empty,
            $"""
            {nameof(CancellationToken)} parameters are exposed as CLI options.
            Use {nameof(ICoconaAppContextAccessor)} instead.

            Violations:
            {string.Join(Environment.NewLine, violations.Select(violation => $"- {violation}"))}
            """
        );
    }

    private static IEnumerable<MethodInfo> DiscoverCommandMethods()
    {
        Type[] controllerTypes =
        [
            ..typeof(BrowseCommandController).Assembly
                .GetTypes()
                .Where(t => t.Namespace?.StartsWith(PresentationNamespace, StringComparison.InvariantCulture) == true)
                .Where(t => t is { IsClass: true, IsPublic: true } && t.Name.EndsWith("Controller", StringComparison.InvariantCulture)),
        ];

        foreach (var controllerType in controllerTypes)
        {
            // Methods with [Command] attribute
            foreach (var method in GetMethodsWithCommandAttribute(controllerType))
            {
                yield return method;
            }

            // Recursively resolve [HasSubCommands] targets
            foreach (var method in GetSubCommandMethods(controllerType))
            {
                yield return method;
            }
        }
    }

    private static IEnumerable<MethodInfo> GetMethodsWithCommandAttribute(Type type)
        => type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<CommandAttribute>() != null);

    private static IEnumerable<MethodInfo> GetSubCommandMethods(Type type)
    {
        foreach (var attr in type.GetCustomAttributes<HasSubCommandsAttribute>())
        {
            var subCommandType = attr.Type;

            // Get public *Async methods from subcommand types
            var asyncMethods = subCommandType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.Name.EndsWith("Async", StringComparison.InvariantCulture));

            foreach (var method in asyncMethods)
            {
                yield return method;
            }

            // Recurse into nested [HasSubCommands]
            foreach (var method in GetSubCommandMethods(subCommandType))
            {
                yield return method;
            }
        }
    }
}
