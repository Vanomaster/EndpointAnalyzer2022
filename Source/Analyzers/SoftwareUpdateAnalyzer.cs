using Analyzers.Base;
using Microsoft.Win32;
using Queries.Database;
using Queries.NonDatabase;
using WGetNET;

namespace Analyzers;

/// <summary>
/// Software analyzer.
/// </summary>
public class SoftwareUpdateAnalyzer : IAnalyzer<List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoftwareUpdateAnalyzer"/> class.
    /// </summary>
    /// <param name="registryParameterValueQuery">Registry parameter value query.</param>
    /// <param name="trustedSoftwareQuery">Trusted Software query.</param>
    public SoftwareUpdateAnalyzer(
        TrustedSoftwareQuery trustedSoftwareQuery, UpgradableSoftwareQuery upgradableSoftware)
    {
        UpgradableSoftwareQuery = upgradableSoftware;
    }

    private UpgradableSoftwareQuery UpgradableSoftwareQuery { get; }

    /// <inheritdoc/>
    public AnalyzeResult<List<string>> Analyze()
    {
        var upgradableSoftwareQueryResult = UpgradableSoftwareQuery.Execute();
        if (!upgradableSoftwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(upgradableSoftwareQueryResult.ErrorMessage);
        }

        var updateRecommendations = GetRecommendations(upgradableSoftwareQueryResult.Data);

        return new AnalyzeResult<List<string>>(updateRecommendations);
    }

    private static List<string> GetRecommendations(List<string> upgradableSoftware)
    {
        var recommendations = new List<string>();

        if (upgradableSoftware.Count != 0)
        {
            recommendations.Add("Были обнаружены обновления следующих программ:\n");
            recommendations.AddRange(upgradableSoftware);
        }
        else
        {
            recommendations.Add(@"Все версии программ актуальны");
        }

        return recommendations;
    }
}