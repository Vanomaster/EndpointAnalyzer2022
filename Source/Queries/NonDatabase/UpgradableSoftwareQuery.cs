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

        // Parse the output of the "winget" command to get the list of updates
        var upgradableSoftware = new List<string>();
        using (var reader = new StreamReader(process.StandardOutput.BaseStream))
        {
            while (!reader.EndOfStream)
            {
                string inputLine = reader.ReadLine();
                string nameLine, currentVersion, availableVersion;
                if (inputLine.Length == 119 || inputLine.Length == 117)
                {
                    nameLine = inputLine.Substring(0, 39);
                    currentVersion = inputLine.Substring(77, 17);
                    availableVersion = inputLine.Substring(94, 17);

                    upgradableSoftware.Add($"{nameLine}|{currentVersion}|{availableVersion}");
                }
            }
        }

        return GetSuccessfulResult(upgradableSoftware);
    }
}