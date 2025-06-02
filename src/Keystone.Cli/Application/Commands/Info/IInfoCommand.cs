using Keystone.Cli.Domain;


namespace Keystone.Cli.Application.Commands.Info;

/// <summary>
/// The "info" command interface.
/// </summary>
public interface IInfoCommand
{
    /// <summary>
    /// Executes the command to produce the info.
    /// </summary>
    /// <returns>
    /// The info to be displayed to the user.
    /// </returns>
    InfoModel GetInfo();
}
