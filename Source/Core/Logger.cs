namespace Core;

/// <summary>
/// Logger.
/// </summary>
public class Logger
{
    private const string LogDirectoryName = @"Журналы";
    private readonly string logDirectoryPath
        = @$"{Constants.UserDirectoryPath}\{Constants.ProgramName}\{LogDirectoryName}";

    /// <summary>
    /// Log.
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
        if (!Directory.Exists(logDirectoryPath))
        {
            Directory.CreateDirectory(logDirectoryPath);
            result += @$"Cоздана директория по пути {logDirectoryPath}." + "\n";
        }

        string fullLogPath = logDirectoryPath + @"\" + fileName;
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