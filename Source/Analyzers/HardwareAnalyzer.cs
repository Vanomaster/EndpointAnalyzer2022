using Analyzers.Base;
using Dal.Entities;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Hardware analyzer.
/// </summary>
public class HardwareAnalyzer : IAnalyzer<List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareAnalyzer"/> class.
    /// </summary>
    /// <param name="hardwareHistoryQuery">Hardware history query.</param>
    /// <param name="trustedHardwareQuery">Trusted hardware query.</param>
    /// <param name="hardwareQuery">Hardware query.</param>
    public HardwareAnalyzer(
        HardwareHistoryQuery hardwareHistoryQuery,
        TrustedHardwareQuery trustedHardwareQuery,
        HardwareQuery hardwareQuery)
    {
        HardwareHistoryQuery = hardwareHistoryQuery;
        TrustedHardwareQuery = trustedHardwareQuery;
        HardwareQuery = hardwareQuery;
    }

    private HardwareHistoryQuery HardwareHistoryQuery { get; }

    private TrustedHardwareQuery TrustedHardwareQuery { get; }

    private HardwareQuery HardwareQuery { get; }

    /// <inheritdoc/>
    public AnalyzeResult<List<string>> Analyze()
    {
        var hardwareHistoryQueryResult = HardwareHistoryQuery.Execute();
        if (!hardwareHistoryQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(hardwareHistoryQueryResult.ErrorMessage);
        }

        var trustedHardwareQueryResult = TrustedHardwareQuery.Execute();
        if (!trustedHardwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(trustedHardwareQueryResult.ErrorMessage);
        }

        var untrustedHardware = GetUntrustedHardware(hardwareHistoryQueryResult.Data, trustedHardwareQueryResult.Data);
        var untrustedHardwareIds = untrustedHardware.Select(hardware => hardware.Id).ToList();
        var hardwareQueryResult = HardwareQuery.Execute(untrustedHardwareIds);
        if (!hardwareQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(hardwareQueryResult.ErrorMessage);
        }

        var recommendations = GetRecommendations(untrustedHardware, hardwareQueryResult.Data);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<RegistryHardware> GetUntrustedHardware(
        IEnumerable<RegistryHardware> validatableCollection,
        ICollection<TrustedHardware> validCollection)
    {
        var invalidCollection = validatableCollection
            .Where(validatableElement => !validCollection
                .Select(validElement => validElement.HardwareId)
                .Contains(validatableElement.Id))
            .ToList();

        return invalidCollection;
    }

    private static List<string> GetRecommendations(
        IReadOnlyCollection<RegistryHardware> untrustedHardwareHistory,
        List<WmiHardware> untrustedHardware)
    {
        var recommendations = new List<string>();
        if (!untrustedHardwareHistory.Any())
        {
            recommendations.Add(@"Недоверенные устройства не обнаружены.");

            return recommendations;
        }

        recommendations.Add(@"Были обнаружены недоверенные устройства."
                            + "\n" + @"Рекомендуется проверить следующие устройства:" + "\n");

        recommendations.Add(@"Подключённые в данный момент недоверенные устройства:" + "\n");

        recommendations.AddRange(untrustedHardware
            .Select(hardware =>
                @"Идентификатор: " + hardware.Id + "\n"
                + @"Название: " + hardware.Name + "\n"
                + @"Описание: " + hardware.Description + "\n"
                + @"Производитель: " + hardware.Manufacturer + "\n"
                + @"Месторасположение: "
                + untrustedHardwareHistory.FirstOrDefault(hardwareHistory => hardwareHistory.Id == hardware.Id)?.Location
                + "\n"));

        recommendations.Add("___________________________________________________________________" + "\n");
        recommendations.Add(@"Все обнаруженные недоверенные устройства:" + "\n");

        recommendations.AddRange(untrustedHardwareHistory
            .Select(hardware =>
                @"Идентификатор: " + hardware.Id + "\n"
                + @"Название: " + hardware.Name + "\n"
                + @"Описание: " + hardware.Description + "\n"
                + @"Производитель: "
                + untrustedHardware.FirstOrDefault(wmiHardware => wmiHardware.Id == hardware.Id)?.Manufacturer + "\n"
                + @"Месторасположение: " + hardware.Location + "\n"
                + @"Дата последнего изменения данных об устройстве: " + hardware.LastHardwareDataModifiedDateTime + "\n"));

        return recommendations;
    }
}