using System.Diagnostics;
using Queries.Base;

namespace Queries.NonDatabase;

/// <inheritdoc />
public class UpgradableSoftwareQuery : NonDbQueryBase<List<string>>
{
    /// <inheritdoc/>
    protected override QueryResult<List<string>> ExecuteCore()
    {
        // Use the "winget" command to get a list of available updates
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "upgrade",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            },
        };

        process.Start();
        var upgradableSoftware = GetUpgradableSoftware(process);

        return GetSuccessfulResult(upgradableSoftware);
    }

    /// <summary>
    /// Parse the output of the "winget" command to get the list of updates.
    /// </summary>
    /// <param name="process">winget process.</param>
    /// <returns>Upgradable software.</returns>
    private static List<string> GetUpgradableSoftware(Process process)
    {
        var upgradableSoftware = new List<string>();
        var indexOfAvailableVersion = 0;
        var indexOfVersion = 0;
        var indexOfId = 0;
        var indexOfSource = 0;
        using var reader = new StreamReader(process.StandardOutput.BaseStream);
        while (!reader.EndOfStream)
        {
            string? inputLine = reader.ReadLine();
            if (inputLine!.Length is not(119 or 117))
            {
                continue;
            }

            if (inputLine.Contains("Версия"))
            {
                indexOfId = inputLine.IndexOf("ИД", StringComparison.Ordinal);
                indexOfVersion = inputLine.IndexOf("Версия", StringComparison.Ordinal);
                indexOfAvailableVersion = inputLine.IndexOf("Доступно", StringComparison.Ordinal);
                indexOfSource = inputLine.IndexOf("Источник", StringComparison.Ordinal);
            }

            string nameLine = inputLine[.. (indexOfId - 1)];
            string currentVersion = inputLine.Substring(indexOfVersion, indexOfAvailableVersion - indexOfVersion);
            string availableVersion = inputLine
                .Substring(indexOfAvailableVersion, indexOfSource - indexOfAvailableVersion);

            upgradableSoftware.Add($"{nameLine}|{currentVersion}|{availableVersion}");
        }

        return upgradableSoftware;
    }
}