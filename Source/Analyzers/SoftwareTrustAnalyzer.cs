using Analyzers.Base;
using Microsoft.Win32;
using Queries.Database;
using Queries.NonDatabase;
using WGetNET;

namespace Analyzers;

/// <summary>
/// Software analyzer.
/// </summary>
public class SoftwareTrustAnalyzer : IAnalyzer<List<SimpleSoftware>, List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoftwareTrustAnalyzer"/> class.
    /// </summary>
    /// <param name="installedSoftwareQuery">Installed software query.</param>
    /// <param name="trustedSoftwareQuery">Trusted Software query.</param>
    public SoftwareTrustAnalyzer(
        InstalledSoftwareQuery installedSoftwareQuery,
        TrustedSoftwareQuery trustedSoftwareQuery)
    {
        InstalledSoftwareQuery = installedSoftwareQuery;
        TrustedSoftwareQuery = trustedSoftwareQuery;
    }

    private InstalledSoftwareQuery InstalledSoftwareQuery { get; }

    private TrustedSoftwareQuery TrustedSoftwareQuery { get; }

    /// <inheritdoc/>
    public AnalyzeResult<List<string>> Analyze(List<SimpleSoftware> model)
    {
        var installedSoftwareQueryResult = InstalledSoftwareQuery.Execute();
        if (!installedSoftwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(installedSoftwareQueryResult.ErrorMessage);
        }

        var trustedSoftwareQueryResult = TrustedSoftwareQuery.Execute();
        if (!trustedSoftwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(trustedSoftwareQueryResult.ErrorMessage);
        }

        List<SimpleSoftware> trustedSoftwareList = new();
        foreach (var trustedSoftware in trustedSoftwareQueryResult.Data)
        {
            Version softwareVersion = null;
            softwareVersion = Version.Parse(trustedSoftware.Version);
            SimpleSoftware trustedSimpleSoftware = new () { Name = trustedSoftware.Name, Version = softwareVersion };
            trustedSoftwareList.Add(trustedSimpleSoftware);
        }

        var untrustedSoftware = GetDifferences(installedSoftwareQueryResult.Data, trustedSoftwareList);

        var recommendations = GetRecommendationsForUntrustedSoftwares(untrustedSoftware);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<SimpleSoftware> GetDifferences(
        List<SimpleSoftware> validatableCollection,
        List<SimpleSoftware> validCollection)
    {
        var invalidSoftware = new List<SimpleSoftware>();
        foreach (var validatableSoftware in validatableCollection)
        {
            var validSoftware = validCollection.FirstOrDefault(element => element.Name == validatableSoftware.Name);
            if (validSoftware == default)
            {
                invalidSoftware.Add(validatableSoftware);
                continue;
            }

            var isValidVersion = validatableSoftware.Version.CompareTo(validSoftware) >= 0;
            if (!isValidVersion)
            {
                invalidSoftware.Add(validatableSoftware);
            }
        }

        return invalidSoftware;
    }

    private static List<string> GetRecommendationsForUntrustedSoftwares(List<SimpleSoftware> untrustedSoftware)
    {
        var recommendations = new List<string>();
        if (!untrustedSoftware.Any())
        {
            recommendations.Add(@"Недоверенные приложения не обнаружены.");

            return recommendations;
        }

        string recommendation = @"Были обнаружены недоверенные приложения."
                                + "\n" + @"Рекомендуется проверить эти приложения:" + "\n";
        recommendation += string.Join("\n", untrustedSoftware);
        recommendations.Add(recommendation);

        return recommendations;
    }
}