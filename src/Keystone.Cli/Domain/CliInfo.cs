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
    /// The current year in local time.
    /// </summary>
    /// <remarks>
    /// Uses local time to match the date script (<c>scripts/get-english-month-year.sh</c>),
    /// which uses <c>date '+%Y'</c> (local time, not UTC).
    /// </remarks>
    public static int CurrentYear => DateTime.Now.Year;
}
