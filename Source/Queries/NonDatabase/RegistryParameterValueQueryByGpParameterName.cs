using Queries.Base;
using Queries.Database;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter value query by gp parameter name.
/// </summary>
public class RegistryParameterValueQueryByGpParameterName : NonDbQueryBase<List<string>?, List<string>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryParameterValueQueryByGpParameterName"/> class.
    /// </summary>
    /// <param name="gpParameterRegistryParametersQueryByGpParameterName">
    /// Group policy parameter registry parameters query by group policy parameter name.</param>
    /// <param name="registryParameterValueQuery">Registry parameter value query.</param>
    public RegistryParameterValueQueryByGpParameterName(
        GpParameterRegistryParametersQueryByGpParameterName gpParameterRegistryParametersQueryByGpParameterName,
        RegistryParameterValueQuery registryParameterValueQuery)
    {
        GpParameterRegistryParametersQueryByGpParameterName = gpParameterRegistryParametersQueryByGpParameterName;
        RegistryParameterValueQuery = registryParameterValueQuery;
    }

    private GpParameterRegistryParametersQueryByGpParameterName GpParameterRegistryParametersQueryByGpParameterName { get; }

    private RegistryParameterValueQuery RegistryParameterValueQuery { get; }

    /// <inheritdoc/>
    protected override QueryResult<List<string>> ExecuteCore(List<string>? gpParameterNames)
    {
        var queryResult = GpParameterRegistryParametersQueryByGpParameterName.Execute(gpParameterNames);
        if (!queryResult.IsSuccessful)
        {
            return GetFailedResult(queryResult.ErrorMessage);
        }

        var registryParameterValueQueryModels = queryResult.Data
            .Select(registryParameter =>
                new RegistryParameterValueQueryModel
                {
                    KeyName = registryParameter.KeyName,
                    ValueName = registryParameter.ParameterName,
                })
            .ToList();

        var registryParameterValueQueryResult = RegistryParameterValueQuery.Execute(registryParameterValueQueryModels);
        if (!registryParameterValueQueryResult.IsSuccessful)
        {
            return GetFailedResult(registryParameterValueQueryResult.ErrorMessage);
        }

        return GetSuccessfulResult(registryParameterValueQueryResult.Data);
    }
}