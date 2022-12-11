using System.Diagnostics;
using Microsoft.Win32;
using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Windows registry parameter value query.
/// </summary>
public class ParametersQueryFromSecedit : NonDbQueryBase<List<SimpleGpParameter>>
{
    /// <inheritdoc/>
    protected override QueryResult<List<SimpleGpParameter>> ExecuteCore()
    {
        var values = new List<SimpleGpParameter>();
        var tempFileSeceditOut = $@"{Directory.GetCurrentDirectory()}\seceditResult.txt";

        var secedit = new Process();
        secedit.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\System32\secedit.exe");
        secedit.StartInfo.Arguments = $@"/export /cfg ""{tempFileSeceditOut}"" /quiet";
        secedit.StartInfo.UseShellExecute = false;
        secedit.StartInfo.CreateNoWindow = true;
        secedit.Start();
        secedit.WaitForExit();

        var readFile = new StreamReader(tempFileSeceditOut);
        var fileReadLines = new List<string>();

        for (int line = 0; line < 27; line++)
        {
            fileReadLines.Add(readFile.ReadLine());
        }

        readFile.Close();
        File.Delete(tempFileSeceditOut);

        foreach (var line in fileReadLines)
        {
            if (!line.Contains(" = ") || line.Contains('"'))
            {
                continue;
            }

            var parameter = new SimpleGpParameter
            {
                RegistryParameterName = line.Substring(0, line.IndexOf(' ')),
                Value = line.Substring(line.LastIndexOf(' ') + 1),
            };

            values.Add(parameter);
        }

        return GetSuccessfulResult(values);
    }
}