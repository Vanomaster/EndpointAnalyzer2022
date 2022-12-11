using Microsoft.Win32;
using Queries.Base;

namespace Queries.NonDatabase;

/// <inheritdoc />
public class InstalledSoftwareQuery : NonDbQueryBase<List<SimpleSoftware>>
{
    private const string RegistryKey1 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

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

        using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView).OpenSubKey(RegistryKey1);

        foreach (var subkeyName in key.GetSubKeyNames())
        {
            using var subkey = key.OpenSubKey(subkeyName);
            if (!IsProgramVisible(subkey!))
            {
                continue;
            }

            var softwareInfo = new SimpleSoftware();
            softwareInfo.Name = (string)subkey.GetValue("DisplayName");
            var softwareVersion = Version.Parse((string)subkey.GetValue("DisplayVersion"));
            softwareInfo.Version = softwareVersion;
            result.Add(softwareInfo);
        }


        return result;
    }

    private static bool IsProgramVisible(RegistryKey subkey)
    {
        var name = (string)subkey.GetValue("DisplayName")!;
        var systemComponentValue = (int?)subkey.GetValue("SystemComponent");
        var isSystemComponent = systemComponentValue > 0;
        var isProgramVisible = !string.IsNullOrEmpty(name) && (!isSystemComponent);

        return isProgramVisible;
    }
}