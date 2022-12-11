using Analyzers.Base;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Hardware analyzer.
/// </summary>
public class HardwareAnalyzer : IAnalyzer<List<RegistryParameterQueryModel>, List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareAnalyzer"/> class.
    /// </summary>
    /// <param name="registryExistentParameterQuery">Registry parameter value query.</param>
    /// <param name="trustedHardwareQuery">Trusted hardware query.</param>
    public HardwareAnalyzer(
        RegistryExistentParameterQuery registryExistentParameterQuery,
        TrustedHardwareQuery trustedHardwareQuery)
    {
        RegistryExistentParameterQuery = registryExistentParameterQuery;
        TrustedHardwareQuery = trustedHardwareQuery;
    }

    private RegistryExistentParameterQuery RegistryExistentParameterQuery { get; }

    private TrustedHardwareQuery TrustedHardwareQuery { get; }

    /// <inheritdoc/>
    public AnalyzeResult<List<string>> Analyze(List<RegistryParameterQueryModel> model)
    {
        var registryParameterValueQueryResult = RegistryExistentParameterQuery.Execute(model);
        if (!registryParameterValueQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(registryParameterValueQueryResult.ErrorMessage);
        }

        var trustedHardwareQueryResult = TrustedHardwareQuery.Execute();
        if (!trustedHardwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(trustedHardwareQueryResult.ErrorMessage);
        }

        var hardwareIds = registryParameterValueQueryResult.Data
            .Select(parameter => parameter.Value);
        var trustedHardwareIds = trustedHardwareQueryResult.Data
            .Select(hardware => hardware.HardwareId)
            .ToList();

        var untrustedHardwareIds = GetDifferences(hardwareIds, trustedHardwareIds);

        var recommendations = GetRecommendations(untrustedHardwareIds);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<string> GetDifferences(
        IEnumerable<string> validatableCollection,
        ICollection<string> validCollection)
    {
        var differences = validatableCollection
            .Where(hardwareId => !validCollection.Contains(hardwareId))
            .ToList();

        return differences;
    }

    private static List<string> GetRecommendations(IReadOnlyCollection<string> differences)
    {
        var recommendations = new List<string>();
        if (!differences.Any())
        {
            recommendations.Add(@"Недоверенные устройства не обнаружены.");

            return recommendations;
        }

        string recommendation = @"Были обнаружены недоверенные устройства."
                                + "\n" + @"Рекомендуется проверить эти устройства:" + "\n";
        recommendation += string.Join("\n", differences);
        recommendations.Add(recommendation);

        return recommendations;
    }
}