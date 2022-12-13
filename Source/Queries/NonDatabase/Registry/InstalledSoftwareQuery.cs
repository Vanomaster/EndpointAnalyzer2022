using Microsoft.Win32;
using Queries.Base;
using System;

namespace Queries.NonDatabase;

/// <inheritdoc />
public class InstalledSoftwareQuery : NonDbQueryBase<List<SimpleSoftware>>
{
    private const string SoftwareRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

    /// <inheritdoc/>
    protected override QueryResult<List<SimpleSoftware>> ExecuteCore()
    {
        var installedSoftware = new List<SimpleSoftware>();
        installedSoftware.AddRange(GetInstalledProgramsFromRegistry(RegistryView.Registry32));
        installedSoftware.AddRange(GetInstalledProgramsFromRegistry(RegistryView.Registry64));

        return GetSuccessfulResult(installedSoftware);
    }

    private static IEnumerable<SimpleSoftware> GetInstalledProgramsFromRegistry(RegistryView registryView)
    {
        var result = new List<SimpleSoftware>();
        using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(SoftwareRegistryKey);
        foreach (string subkeyName in key.GetSubKeyNames())
        {
            using var subkey = key.OpenSubKey(subkeyName);
            if (!IsVisibleProgram(subkey!))
            {
                continue;
            }

            var software = new SimpleSoftware
            {
                Name = subkey.GetValue("DisplayName")?.ToString(),
            };

            if (Version.TryParse(subkey.GetValue("DisplayVersion")?.ToString(), out var softwareVersion))
            {
                software.Version = softwareVersion;
            }

            result.Add(software);
        }

        return result;
    }

    private static bool IsVisibleProgram(RegistryKey subkey)
    {
        var name = subkey.GetValue("DisplayName")?.ToString();
        var systemComponentValue = subkey.GetValue("SystemComponent") as int?;
        bool isSystemComponent = systemComponentValue > 0;
        bool isVisibleProgram = !string.IsNullOrEmpty(name) && (!isSystemComponent);

        return isVisibleProgram;
    }
}