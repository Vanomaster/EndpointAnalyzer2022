namespace Commands.Base;

/// <summary>
/// Command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Execute command.
    /// </summary>
    /// <returns>Command result.</returns>
    CommandResult Execute();
}