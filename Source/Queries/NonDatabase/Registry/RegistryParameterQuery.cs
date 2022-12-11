using Microsoft.Win32;
using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter value query.
/// </summary>
public class RegistryParameterQuery : NonDbQueryBase<List<RegistryParameterQueryModel>, List<RegistryParameter>>
{
    /// <summary>
    /// The value to return if valueName does not exist.
    /// </summary>
    private const string DefaultValue = "_______________";

    /// <inheritdoc/>
    protected override QueryResult<List<RegistryParameter>> ExecuteCore(
        List<RegistryParameterQueryModel> models)
    {
        var registryParameters = new List<RegistryParameter>();
        foreach (var model in models)
        {
            var value = Registry.GetValue(model.KeyName, model.ParameterName, DefaultValue)?.ToString();

            var parameter = new RegistryParameter
            {
                KeyName = model.KeyName,
                ParameterName = model.ParameterName,
                Value = value,
            };

            registryParameters.Add(parameter);
        }

        return GetSuccessfulResult(registryParameters);
    }
}