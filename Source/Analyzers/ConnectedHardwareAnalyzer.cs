using Analyzers.Base;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Connected hardware analyzer.
/// </summary>
public class ConnectedHardwareAnalyzer : IAnalyzer<string, bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareAnalyzer"/> class.
    /// </summary>
    /// <param name="trustedHardwareQuery">Trusted hardware query.</param>
    public ConnectedHardwareAnalyzer(TrustedHardwareQuery trustedHardwareQuery)
    {
        TrustedHardwareQuery = trustedHardwareQuery;
    }

    private TrustedHardwareQuery TrustedHardwareQuery { get; }

    /// <inheritdoc/>
    public AnalyzeResult<bool> Analyze(string validatableHardwareId)
    {
        var trustedHardwareQueryResult = TrustedHardwareQuery.Execute();
        if (!trustedHardwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<bool>(trustedHardwareQueryResult.ErrorMessage);
        }

        bool isTrustedHardware = trustedHardwareQueryResult.Data
            .Select(hardware => hardware.HardwareId)
            .Contains(validatableHardwareId);

        return new AnalyzeResult<bool>(isTrustedHardware);
    }
}