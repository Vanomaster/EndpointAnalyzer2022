using Analyzers.Base;
using Microsoft.SqlServer.Server;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Software analyzer.
/// </summary>
public class SoftwareTrustAnalyzer : IAnalyzer<List<string>>
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
    public AnalyzeResult<List<string>> Analyze()
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

        List<SimpleSoftware> trustedSoftwareList = new ();

        foreach (var trustedSoftware in trustedSoftwareQueryResult.Data)
        {
            var softwareVersion = Version.Parse(trustedSoftware.Version);
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

            if ((validatableSoftware.Version == null && validSoftware.Version != null) ||
                (validatableSoftware.Version != null && validSoftware.Version == null))
            {
                invalidSoftware.Add(validatableSoftware);
            }

            if (validatableSoftware.Version == null || validSoftware.Version == null)
            {
                continue;
            }

            var isValidVersion = validatableSoftware.Version.CompareTo(validSoftware.Version) >= 0;
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

        recommendations.Add(@"Были обнаружены недоверенные приложения."
                            + "\n" + @"Рекомендуется проверить эти приложения:" + "\n");
        foreach (var software in untrustedSoftware)
        {
            recommendations.Add($"{CutSoftwareString(software.Name)} | {CutSoftwareString(software.Version?.ToString())}");
        }

        return recommendations;
    }

    private static string CutSoftwareString(string? inputString)
    {
        if (inputString != null && inputString.Length >= 55)
        {
            return $"{inputString.Substring(0, 53) + "..",-55}";
        }
        else
        {
            return $"{inputString,-55}";
        }
    }
}