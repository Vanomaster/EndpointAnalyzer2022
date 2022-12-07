namespace Analyzers.Base;

/// <summary>
/// Analyzer.
/// </summary>
/// <typeparam name="TModel">Model of analyze.</typeparam>
/// <typeparam name="TResult">Result of analyze.</typeparam>
public interface IAnalyzer<in TModel, TResult>
{
    /// <summary>
    /// Analyze.
    /// </summary>
    /// <param name="model">Analyze model.</param>
    /// <returns>Analyze result model.</returns>
    AnalyzeResult<TResult> Analyze(TModel model);
}