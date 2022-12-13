using Analyzers.Base;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Software trust analyzer.
/// </summary>
public class SoftwareTrustAnalyzer : IAnalyzer<List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoftwareTrustAnalyzer"/> class.
    /// </summary>
    /// <param name="installedSoftwareQuery">Installed software query.</param>
    /// <param name="trustedSoftwareQuery">Trusted software query.</param>
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

        var untrustedSoftware = GetUntrustedSoftware(installedSoftwareQueryResult.Data, trustedSoftwareQueryResult.Data);
        var recommendations = GetRecommendations(untrustedSoftware);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<SimpleSoftware> GetUntrustedSoftware(
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

            bool isValidVersion = validatableSoftware.Version.CompareTo(validSoftware.Version) >= 0;
            if (!isValidVersion)
            {
                invalidSoftware.Add(validatableSoftware);
            }
        }

        return invalidSoftware;
    }

    private static List<string> GetRecommendations(List<SimpleSoftware> untrustedSoftware)
    {
        var recommendations = new List<string>();
        if (!untrustedSoftware.Any())
        {
            recommendations.Add(@"Недоверенные приложения не обнаружены.");

            return recommendations;
        }

        recommendations.Add(@"Были обнаружены недоверенные приложения."
                            + "\n" + @"Рекомендуется проверить следующие приложения:" + "\n");

        recommendations.AddRange(untrustedSoftware
            .Select(software =>
                $"{GetCutSoftwareName(software.Name)} | {GetCutSoftwareName(software.Version?.ToString())}"));

        return recommendations;
    }

    private static string GetCutSoftwareName(string? inputString)
    {
        if (inputString != null && inputString.Length >= 55)
        {
            return $"{inputString[..53] + "..",-55}";
        }

        return $"{inputString,-55}";
    }
}