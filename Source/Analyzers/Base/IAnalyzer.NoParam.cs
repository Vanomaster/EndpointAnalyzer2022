namespace Analyzers.Base;

/// <summary>
/// Analyzer.
/// </summary>
/// <typeparam name="TResult">Result of analyze.</typeparam>
public interface IAnalyzer<TResult>
{
    /// <summary>
    /// Analyze.
    /// </summary>
    /// <returns>Analyze result model.</returns>
    AnalyzeResult<TResult> Analyze();
}