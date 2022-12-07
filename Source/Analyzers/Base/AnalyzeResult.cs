using Analyzers.Base;
using Queries.Base;

namespace Analyzers;

/// <inheritdoc />
public class AnalyzeResult<TResult> : IAnalyzeResult<TResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{TResult}"/> class.
    /// </summary>
    /// <param name="data">.</param>
    public AnalyzeResult(TResult data)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
    /// </summary>
    /// <param name="errorMessage">Message of error.</param>
    public AnalyzeResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc/>
    public TResult Data { get; } = default!;

    /// <inheritdoc/>
    public string ErrorMessage { get; } = null!;

    /// <inheritdoc/>
    public bool IsSuccessful => ErrorMessage == null;
}