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
                Value = parameter.Value?.ToString(),
            });

        var gpParameters = parametersQueryFromSeceditResult.Data;
        gpParameters.AddRange(registryGpParameters);
        var gpParametersRecommendations = gpParametersRecommendationsQueryResult.Data;
        var invalidParameters = GetInvalidParameters(gpParameters, gpParametersRecommendations);
        var nonexistentRegistryGpParameter = registryParameters.RegistryNonexistentParameters
            .Select(parameter => new SimpleGpParameter
            {
                RegistryParameterName = parameter.ParameterName,
                Value = parameter.Value?.ToString(),
            })
            .ToList();

        var recommendations = GetRecommendations(invalidParameters, nonexistentRegistryGpParameter);

        return new AnalyzeResult<List<string>>(recommendations);
    }

    private static List<SimpleGpParameter> GetInvalidParameters(
        List<SimpleGpParameter> validatableCollection,
        List<SimpleGpParameter> validCollection)
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

            bool isValidParameter = validParameter.Direction switch
            {
                null => validatableParameter.Value == validParameter.Value,
                false => int.Parse(validatableParameter.Value) <= int.Parse(validParameter.Value),
                true => int.Parse(validatableParameter.Value) >= int.Parse(validParameter.Value),
            };

            if (validParameter.Direction == false && validParameter.Value == "0")
            {
                invalidParameters.Add(validatableParameter);
            }

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
        var recommendations = new List<string>();
        if (!invalidParameters.Any() && !nonexistentParameters.Any())
        {
            recommendations.Add(@"Параметры, значения которых не соответствуют рекомендуемым значениям, не обнаружены.");

            return recommendations;
        }

        invalidParameters.AddRange(nonexistentParameters);
        var invalidParametersNames = invalidParameters
            .Select(parameter => parameter.RegistryParameterName)
            .ToList();

        var gpParametersRationalesRecommendationsQueryResult =
            GpParametersRationalesRecommendationsQuery.Execute(invalidParametersNames);
        if (!gpParametersRationalesRecommendationsQueryResult.IsSuccessful)
        {
            return new List<string> { gpParametersRationalesRecommendationsQueryResult.ErrorMessage };
        }

        var rationalesRecommendations = gpParametersRationalesRecommendationsQueryResult.Data;
        var nonexistentParametersNames = nonexistentParameters
            .Select(nonexistentPar => nonexistentPar.RegistryParameterName)
            .ToList();

        recommendations.Add(@"Были обнаружены параметры, значения которых не соответствуют рекомендуемым значениям."
                            + "\n" + @"Рекомендуется исправить значения следующих параметров:" + "\n");

        foreach (var invalidParameter in invalidParameters)
        {
            var parameterRationale = rationalesRecommendations
                .FirstOrDefault(rec => rec.RegistryParemeterName == invalidParameter.RegistryParameterName);

            string recommendation = @$"Имя параметра: {parameterRationale?.Name}" + "\n";

            if (nonexistentParametersNames.Contains(invalidParameter.RegistryParameterName))
            {
                recommendation += @"Текущее значение параметра: Не установлено" + "\n";
            }
            else
            {
                recommendation += @$"Текущее значение параметра: {invalidParameter.Value}" + "\n";
            }

            recommendation += parameterRationale.Direction switch
            {
                null => @$"Рекомендуемое значение параметра: {parameterRationale.Value}" + "\n",
                false => @$"Рекомендуемое значение параметра: не более {parameterRationale.Value}" + "\n",
                true => @$"Рекомендуемое значение параметра: не менее {parameterRationale.Value}" + "\n",
            };

            recommendation += @$"Описание параметра: {parameterRationale.Description}" + "\n"
                   + @$"Обоснование параметра: {parameterRationale.Rationale}" + "\n"
                   + @$"Влияние параметра на систему: {parameterRationale.Impact}" + "\n"
                   + "_____________________________________________________________________________________" + "\n";

            recommendations.Add(recommendation);
        }

        return recommendations;
    }
}