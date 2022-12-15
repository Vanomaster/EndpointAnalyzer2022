using Microsoft.Win32;
using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter that can be accessed query.
/// </summary>
public class RegistryParametersQueryByKeyName : NonDbQueryBase<List<string>, List<RegistryParameter>>
{
    /// <inheritdoc/>
    protected override QueryResult<List<RegistryParameter>> ExecuteCore(List<string> keyNames)
    {
        var registryParameters = new List<RegistryParameter>();
        foreach (string keyName in keyNames)
        {
            var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey(keyName);
            if (key is null)
            {
                return GetFailedResult(@$"Opening subkey {keyName} was failed.");
            }

            foreach (string subKeyName in key.GetSubKeyNames())
            {
                var subKey = GetSubKey(key, subKeyName);
                if (subKey is null)
                {
                    continue;
                }

                var parameters = GetRegistryParameters(subKey);
                foreach (var parameter in parameters)
                {
                    var registryKeyEntity = new RegistryKeyEntity(subKey);
                    parameter.KeyName = subKey.Name;
                    parameter.LastWriteDateTime = registryKeyEntity.LastWriteTime;
                }

                registryParameters.AddRange(parameters);
            }
        }

        return GetSuccessfulResult(registryParameters);
    }

    private static List<RegistryParameter> GetRegistryParameters(RegistryKey key)
    {
        var registryParameters = new List<RegistryParameter>();
        foreach (string valueName in key.GetValueNames())
        {
            var registryParameter = new RegistryParameter()
            {
                ParameterName = valueName,
            };

            if (key.GetValueKind(valueName) == RegistryValueKind.MultiString)
            {
                registryParameter.Value = key.GetValue(valueName) as string[];
            }

            if (key.GetValueKind(valueName) != RegistryValueKind.MultiString)
            {
                registryParameter.Value = key.GetValue(valueName)?.ToString();
            }

            registryParameters.Add(registryParameter);
        }

        foreach (string subKeyName in key.GetSubKeyNames())
        {
            var subKey = GetSubKey(key, subKeyName);
            if (subKey is null)
            {
                continue;
            }

            registryParameters.AddRange(GetRegistryParameters(subKey));
        }

        return registryParameters;
    }

    private static RegistryKey? GetSubKey(RegistryKey key, string subKeyName)
    {
        try
        {
            var subKey = key.OpenSubKey(subKeyName);

            return subKey;
        }
        catch (Exception)
        {
            return null;
        }
    }
}