using Microsoft.Win32;
using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter value query.
/// </summary>
public class RegistryParameterValueQuery : NonDbQueryBase<List<RegistryParameterValueQueryModel>, List<string>>
{
    /// <summary>
    /// The value to return if valueName does not exist.
    /// </summary>
    private const string DefaultValue = "_______________";

    /// <inheritdoc/>
    protected override QueryResult<List<string>> ExecuteCore(List<RegistryParameterValueQueryModel> models)
    {
        var values = new List<string>();
        foreach (var model in models)
        {
            var value = Registry.GetValue(model.KeyName, model.ValueName, DefaultValue)?.ToString();
            if (value is null)
            {
                return GetFailedResult(@$"subkey {model.KeyName} specified by keyName does not exist.");
            }

            if (value == DefaultValue)
            {
                return GetFailedResult($"valueName {model.ValueName} does not exist.");
            }

            values.Add(value);
        }

        return GetSuccessfulResult(values);
    }
}