using Queries.Base;
using Queries.Database;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter value query by gp parameter name.
/// </summary>
public class RegistryParameterQueryByGpParameterName : NonDbQueryBase<List<string>?, RegistryParameters>
{
    /// <summary>
    /// The value to return if valueName does not exist.
    /// </summary>
    private const string DefaultValue = "_______________";

    private const string NonRegistryKeyName = "Non registry. Account policy";

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryParameterQueryByGpParameterName"/> class.
    /// </summary>
    /// <param name="gpRegistryParameterQueryByGpParameterName">
    /// Group policy parameter registry parameters query by group policy parameter name.</param>
    /// <param name="registryParameterQuery">Registry parameter query.</param>
    public RegistryParameterQueryByGpParameterName(
        GpRegistryParameterQueryByGpParameterName gpRegistryParameterQueryByGpParameterName,
        RegistryParameterQuery registryParameterQuery)
    {
        GpRegistryParameterQueryByGpParameterName = gpRegistryParameterQueryByGpParameterName;
        RegistryParameterQuery = registryParameterQuery;
    }

    private GpRegistryParameterQueryByGpParameterName GpRegistryParameterQueryByGpParameterName { get; }

    private RegistryParameterQuery RegistryParameterQuery { get; }

    /// <inheritdoc/>
    protected override QueryResult<RegistryParameters> ExecuteCore(List<string>? gpParameterNames)
    {
        var gpRegistryParameterQueryResult = GpRegistryParameterQueryByGpParameterName.Execute(gpParameterNames);
        if (!gpRegistryParameterQueryResult.IsSuccessful)
        {
            return GetFailedResult(gpRegistryParameterQueryResult.ErrorMessage);
        }

        gpRegistryParameterQueryResult.Data.RemoveAll(parameter => parameter.KeyName == NonRegistryKeyName);

        var registryParameterQueryModels = gpRegistryParameterQueryResult.Data
            .Select(gpRegistryParameter =>
                new RegistryParameterQueryModel
                {
                    KeyName = gpRegistryParameter.KeyName,
                    ParameterName = gpRegistryParameter.ParameterName,
                })
            .ToList();

        var registryParameterQueryResult = RegistryParameterQuery.Execute(registryParameterQueryModels);
        if (!registryParameterQueryResult.IsSuccessful)
        {
            return GetFailedResult(registryParameterQueryResult.ErrorMessage);
        }

        var separatedByExistenceRegistryParameters =
            GetSeparatedByExistenceRegistryParameters(registryParameterQueryResult.Data);

        return GetSuccessfulResult(separatedByExistenceRegistryParameters);
    }

    private static RegistryParameters GetSeparatedByExistenceRegistryParameters(List<RegistryParameter> registryParameters)
    {
        var registryNonexistentParameters = new List<RegistryParameter>();
        var registryExistentParameters = new List<RegistryParameter>();
        foreach (var registryParameter in registryParameters)
        {
            if (registryParameter.Value is null or DefaultValue)
            {
                registryNonexistentParameters.Add(registryParameter);
                continue;
            }

            registryExistentParameters.Add(registryParameter);
        }

        var separatedByExistenceRegistryParameters = new RegistryParameters
        {
            RegistryNonexistentParameters = new List<RegistryParameter>(registryNonexistentParameters),
            RegistryExistentParameters = new List<RegistryParameter>(registryExistentParameters),
        };

        return separatedByExistenceRegistryParameters;
    }
}