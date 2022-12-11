using Dal;
using Microsoft.EntityFrameworkCore;

namespace Commands.Base;

/// <inheritdoc />
public abstract class CommandBase<TArgs, TResult> : ICommand<TArgs, TResult>
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
    public CommandResult<TResult> Execute(TArgs args)
    {
        try
        {
            Context = ContextFactory.CreateDbContext();
            return ExecuteCore(args);
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
    /// <param name="args">Command arguments.</param>
    /// <returns>Query result.</returns>
    protected abstract CommandResult<TResult> ExecuteCore(TArgs args);

    /// <summary>
    /// Get successful result.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <returns>Command result.</returns>
    protected CommandResult<TResult> GetSuccessfulResult(TResult data)
    {
        return new CommandResult<TResult>(data: data);
    }

    /// <summary>
    /// Get failed result.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <returns>Command result.</returns>
    protected CommandResult<TResult> GetFailedResult(string errorMessage)
    {
        return new CommandResult<TResult>(errorMessage);
    }
}