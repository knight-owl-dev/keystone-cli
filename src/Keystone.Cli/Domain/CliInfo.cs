namespace Keystone.Cli.Domain;

/// <summary>
/// Metadata about the Keystone CLI application itself.
/// </summary>
public static class CliInfo
{
    /// <summary>
    /// The name of the Keystone CLI application.
    /// </summary>
    public const string AppName = "keystone-cli";

    /// <summary>
    /// The year the Keystone CLI was created.
    /// </summary>
    public const int InceptionYear = 2025;

    /// <summary>
    /// Returns the current local date.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses local time to match the date script (<c>scripts/get-english-month-year.sh</c>),
    /// which uses <c>date</c> without UTC flags.
    /// </para>
    /// <para>
    /// Returning a single <see cref="DateOnly"/> ensures atomic access to year and month,
    /// avoiding race conditions at year/month boundaries.
    /// </para>
    /// </remarks>
    public static DateOnly GetLocalDate()
        => DateOnly.FromDateTime(DateTime.Now);
}
