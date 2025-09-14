namespace Keystone.Cli.Application.Utility;

/// <summary>
/// Functional extensions for various types enable the use of functional programming concepts.
/// </summary>
public static class FunctionalExtensions
{
    /// <summary>
    /// Executes the specified <paramref name="action"/> on the given <paramref name="instance"/>
    /// and returns the same instance. This is useful for performing side effects in a fluent manner.
    /// </summary>
    /// <param name="instance">The source instance.</param>
    /// <param name="action">The action to execute.</param>
    /// <typeparam name="T">The type of instance.</typeparam>
    /// <returns>
    /// The original <paramref name="instance"/> after executing the <paramref name="action"/>.
    /// </returns>
    public static T With<T>(this T instance, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action(instance);

        return instance;
    }
}
