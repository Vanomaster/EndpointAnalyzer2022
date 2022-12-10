using Microsoft.Win32;
using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter value query.
/// </summary>
public class RegistryExistentParameterQuery : NonDbQueryBase<List<RegistryParameterQueryModel>, List<RegistryParameter>>
{
    /// <summary>
    /// The value to return if valueName does not exist.
    /// </summary>
    private const string DefaultValue = "_______________";

    /// <inheritdoc/>
    protected override QueryResult<List<RegistryParameter>> ExecuteCore(
        List<RegistryParameterQueryModel> models)
    {
        var values = new List<RegistryParameter>();
        foreach (var model in models)
        {
            var value = Registry.GetValue(model.KeyName, model.ParameterName, DefaultValue)?.ToString();
            if (value is null)
            {
                return GetFailedResult(@$"subkey {model.KeyName} specified by keyName does not exist.");
            }

            if (value == DefaultValue)
            {
                return GetFailedResult($"valueName {model.ParameterName} does not exist.");
            }

            var parameter = new RegistryParameter
            {
                ParameterName = model.ParameterName,
                Value = value,
            };

            values.Add(parameter);
        }

        return GetSuccessfulResult(values);
    }
}