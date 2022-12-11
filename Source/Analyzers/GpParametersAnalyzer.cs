using Analyzers.Base;
using Queries.Database;
using Queries.NonDatabase;

namespace Analyzers;

/// <summary>
/// Hardware analyzer.
/// </summary>
public class GpParametersAnalyzer : IAnalyzer<List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareAnalyzer"/> class.
    /// </summary>
    /// <param name="registryParameterQueryByGpParameterName">Registry parameter value query.</param>
    /// <param name="trustedHardwareQuery">Trusted hardware query.</param>
    public GpParametersAnalyzer(
        RegistryParameterQueryByGpParameterName registryParameterQueryByGpParameterName,
        GpParametersValuesRecommendationsQuery gpParametersValuesRecommendationsQuery,
        ParametersQueryFromSecedit parametersQueryFromSecedit,
        GpParametersRationalesRecommendationsQuery gpParametersRationalesRecommendationsQuery)
    {
        RegistryParameterQueryByGpParameterName = registryParameterQueryByGpParameterName;
        GpParametersValuesRecommendationsQuery = gpParametersValuesRecommendationsQuery;
        ParametersQueryFromSecedit = parametersQueryFromSecedit;
        GpParametersRationalesRecommendationsQuery = gpParametersRationalesRecommendationsQuery;
    }

    private RegistryParameterQueryByGpParameterName RegistryParameterQueryByGpParameterName { get; }

    private ParametersQueryFromSecedit ParametersQueryFromSecedit { get; }

    private GpParametersValuesRecommendationsQuery GpParametersValuesRecommendationsQuery { get; }

    private GpParametersRationalesRecommendationsQuery GpParametersRationalesRecommendationsQuery { get; }

    /// <inheritdoc/>
    public AnalyzeResult<List<string>> Analyze()
    {
        var registryParameterValueQueryResult = RegistryParameterQueryByGpParameterName.Execute();
        if (!registryParameterValueQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(registryParameterValueQueryResult.ErrorMessage);
        }

        var parametersQueryFromSeceditResult = ParametersQueryFromSecedit.Execute();
        if (!parametersQueryFromSeceditResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(registryParameterValueQueryResult.ErrorMessage);
        }

        var gpParametersRecommendationsQueryResult = GpParametersValuesRecommendationsQuery.Execute();
        if (!gpParametersRecommendationsQueryResult.IsSuccessful)
        {
            return new AnalyzeResult<List<string>>(gpParametersRecommendationsQueryResult.ErrorMessage);
        }

        var registryParameters = registryParameterValueQueryResult.Data;

        var registryGpParameters = registryParameters.RegistryExistentParameters
            .Select(parameter => new SimpleGpParameter
            {
                RegistryParameterName = parameter.ParameterName,
                Value = parameter.Value,
            });

        var gpParameters = parametersQueryFromSeceditResult.Data;
        gpParameters.AddRange(registryGpParameters);
        var gpParametersRecommendations = gpParametersRecommendationsQueryResult.Data;
        var invalidParameters =
            GetDifferences(gpParameters, gpParametersRecommendations);

        var nonexistentRegistryGpParameter = registryParameters.RegistryNonexistentParameters
            .Select(parameter => new SimpleGpParameter
            {
                RegistryParameterName = parameter.ParameterName,
                Value = parameter.Value,
            })
            .ToList();

        var recommendations = GetRecommendations(invalidParameters, nonexistentRegistryGpParameter);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<SimpleGpParameter> GetDifferences(
        IEnumerable<SimpleGpParameter> validatableCollection,
        ICollection<SimpleGpParameter> validCollection)
    {
        var invalidParameters = new List<SimpleGpParameter>();
        foreach (var validatableParameter in validatableCollection)
        {
            var validParameter = validCollection
                .FirstOrDefault(parameter =>
                    parameter.RegistryParameterName == validatableParameter.RegistryParameterName);

            if (validParameter == default)
            {
                continue; // Если в БД нет считанных параметров
            }

            var isValidParameter = validParameter.Direction switch
            {
                null => validatableParameter.Value == validParameter.Value,
                false => int.Parse(validatableParameter.Value) <= int.Parse(validParameter.Value),
                true => int.Parse(validatableParameter.Value) >= int.Parse(validParameter.Value),
            };

            if (!isValidParameter)
            {
                invalidParameters.Add(validatableParameter);
            }
        }

        return invalidParameters;
    }

    private List<string> GetRecommendations(
        List<SimpleGpParameter> invalidParameters,
        List<SimpleGpParameter> nonexistentParameters)
    {
        var badParameters = invalidParameters;
        badParameters.AddRange(nonexistentParameters);
        var invalidParametersNames = badParameters
            .Select(parameter => parameter.RegistryParameterName)
            .ToList();

        var gpParametersRationalesRecommendationsQueryResult =
            GpParametersRationalesRecommendationsQuery.Execute(invalidParametersNames);

        if (!gpParametersRationalesRecommendationsQueryResult.IsSuccessful)
        {
            //return new AnalyzeResult<List<string>>(gpParametersRationalesRecommendationsQueryResult.ErrorMessage);
        }

        var rationalesRecommendations = gpParametersRationalesRecommendationsQueryResult.Data;
        var recommendations = new List<string>();


        var nonexistentParametersNames = nonexistentParameters
            .Select(nonexistentPar => nonexistentPar.RegistryParameterName);

        foreach (var badParameter in badParameters)
        {
            var parameterRationale = rationalesRecommendations
                .FirstOrDefault(rec => rec.RegistryParemeterName == badParameter.RegistryParameterName);

            string rec = $@"Имя параметра: {parameterRationale.Name}\n";

            if (nonexistentParametersNames.Contains(badParameter.RegistryParameterName))
            {
                rec += @"Текущее значение параметра: Не установлено\n";
            }
            else
            {
                rec += $@"Текущее значение параметра: {badParameter.Value}\n";
            }

            rec += parameterRationale.Direction switch
            {
                null => $@"Рекомендуемое значение параметра: {parameterRationale.Value}\n",
                false => $@"Рекомендуемое значение параметра: не более {parameterRationale.Value}\n",
                true => $@"Рекомендуемое значение параметра: не менее {parameterRationale.Value}\n",
            };

            rec += $@"Описание параметра: {parameterRationale.Description}\n"
                   + $@"Обоснование параметра: {parameterRationale.Rationale}\n"
                   + $@"Влияние параметра на систему: {parameterRationale.Impact}\n"
                   + $@"_____________________________________________________________\n\n";

            recommendations.Add(rec);
        }

        return recommendations;
    }
}