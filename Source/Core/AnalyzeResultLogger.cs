namespace Core;

/// <summary>
/// Analyze result logger.
/// </summary>
public class AnalyzeResultLogger
{
    private const string LogDirectoryName = @"Журналы";
    private static readonly string LogDirectoryPath = Directory.GetCurrentDirectory();
    private readonly string logPath = @$"{LogDirectoryPath}\{LogDirectoryName}";

    /// <summary>
    /// Log analyze result.
    /// </summary>
    /// <param name="data">Data to log.</param>
    /// <param name="fileName">Log file name.</param>
    /// <returns>Log result.</returns>
    public string Log(List<string> data, string fileName)
    {
        try
        {
            string result = WriteToFile(data, fileName);

            return result;
        }
        catch (Exception exception)
        {
            return @"Не удалось записать результат анализа в файл из-за ошибки." + "\n" + $"{exception.Message}";
        }
    }

    private string WriteToFile(List<string> data, string fileName)
    {
        var result = string.Empty;
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
            result += @$"Cоздана директория по пути {logPath}." + "\n";
        }

        string fullLogPath = logPath + @"\" + fileName;
        if (File.Exists(fullLogPath))
        {
            AddDataToFileBeginning(data, fullLogPath);
        }

        if (!File.Exists(fullLogPath))
        {
            string dateTime = "\n" + @$"{DateTime.Now}.{DateTime.Now.Millisecond}" + "\n";
            var dataToLog = new List<string> { dateTime };
            dataToLog.AddRange(data);
            File.WriteAllLines(fullLogPath, dataToLog);
            result += @$"Cоздан файл по пути {fullLogPath}." + "\n";
        }

        result += @$"Результат анализа записан в файл по пути {fullLogPath}.";

        return result;
    }

    private static void AddDataToFileBeginning(List<string> data, string fullLogPath)
    {
        string oldText = File.ReadAllText(fullLogPath);
        using var writer = new StreamWriter(fullLogPath, false);
        writer.WriteLine("\n" + @$"{DateTime.Now}.{DateTime.Now.Millisecond}" + "\n");
        foreach (string line in data)
        {
            writer.WriteLine(line);
        }

        writer.WriteLine("\n");
        writer.WriteLine(oldText);
    }
}