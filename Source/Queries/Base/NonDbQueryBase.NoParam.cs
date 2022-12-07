namespace Queries.Base;

/// <inheritdoc />
public abstract class NonDbQueryBase<TResult> : IQuery<TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryBase{TResult}"/> class.
    /// </summary>
    protected NonDbQueryBase()
    {
    }

    /// <inheritdoc/>
    public QueryResult<TResult> Execute()
    {
        try
        {
            return ExecuteCore();
        }
        catch (Exception exception)
        {
            return GetFailedResult(exception.ToString());
        }
    }

    /// <summary>
    /// Execute query core.
    /// </summary>
    /// <returns>Query result.</returns>
    protected abstract QueryResult<TResult> ExecuteCore();

    /// <summary>
    /// Get successful result.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <returns>Query result.</returns>
    protected QueryResult<TResult> GetSuccessfulResult(TResult data)
    {
        return new QueryResult<TResult>(data: data);
    }

    /// <summary>
    /// Get failed result.
    /// </summary>
    /// <param name="errorMessage">Error message.</param>
    /// <returns>Query result.</returns>
    protected QueryResult<TResult> GetFailedResult(string errorMessage)
    {
        return new QueryResult<TResult>(errorMessage);
    }
}