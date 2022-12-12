using Dal;
using Microsoft.EntityFrameworkCore;

namespace Commands.Base;

/// <inheritdoc />
public abstract class CommandBase : ICommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase{TArgs}"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    protected CommandBase(IDbContextFactory<Context> contextFactory)
    {
        ContextFactory = contextFactory;
    }

    /// <summary>
    /// Context.
    /// </summary>
    protected Context Context { get; private set; } = null!;

    private IDbContextFactory<Context> ContextFactory { get; }

    /// <inheritdoc/>
    public CommandResult Execute()
    {
        try
        {
            Context = ContextFactory.CreateDbContext();
            return ExecuteCore();
        }
        catch (Exception exception)
        {
            return GetFailedResult(exception.ToString());
        }
        finally
        {
            if (Context is not null)
            {
                Context.Dispose();
            }
        }
    }

    /// <summary>
    /// Execute command core.
    /// </summary>
    /// <returns>Query result.</returns>
    protected abstract CommandResult ExecuteCore();

    /// <summary>
    /// Get successful result.
    /// </summary>
    /// <returns>Command result.</returns>
    protected CommandResult GetSuccessfulResult()
    {
        return new CommandResult();
    }

    /// <summary>
    /// Get failed result.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <returns>Command result.</returns>
    protected CommandResult GetFailedResult(string errorMessage)
    {
        return new CommandResult(errorMessage);
    }
}