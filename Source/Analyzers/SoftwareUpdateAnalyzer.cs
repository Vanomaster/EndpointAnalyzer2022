using Analyzers.Base;
using Microsoft.Win32;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Software update analyzer.
/// </summary>
public class SoftwareUpdateAnalyzer : IAnalyzer<List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoftwareUpdateAnalyzer"/> class.
    /// </summary>
    /// <param name="upgradableSoftware">Upgradable software query.</param>
    public SoftwareUpdateAnalyzer(UpgradableSoftwareQuery upgradableSoftware)
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

        var recommendations = GetRecommendations(upgradableSoftwareQueryResult.Data);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<string> GetRecommendations(List<string> upgradableSoftware)
    {
        var recommendations = new List<string>();
        if (!upgradableSoftware.Any())
        {
            recommendations.Add(@"Доступные обновления не обнаружены.");

            return recommendations;
        }

        recommendations.Add(@"Были обнаружены обновления для следующих программ:" + "\n");
        recommendations.AddRange(upgradableSoftware);

        return recommendations;
    }
}