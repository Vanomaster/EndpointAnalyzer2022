using Commands;
using Commands.Base;
using Microsoft.Extensions.DependencyInjection;
using Queries.NonDatabase;

namespace Core;

/// <summary>
/// Database updater from CSV file.
/// </summary>
public class DbUpdaterFromScvFiles
{
    private const string DbDirectoryName = "База данных";
    private const string GpParametersDirectoryName = @"Параметры групповых политик";
    private const string SoftwareDirectoryName = @"Программное обеспечение";
    private const string HardwareDirectoryName = @"Устройства";
    private const string FoundPathsMessage = "Найденные пути:\n";
    private const string SuccessfullyMessage = " в базе данных успешно обновлены.\n";
    private static readonly string SourceDbDirectoryPath = @$"{Constants.ProgramExePath}\..\..\..\..\..\Db";
    private static readonly string UserDbDirectoryPath
        = @$"{Constants.UserDirectoryPath}\{Constants.ProgramName}\{DbDirectoryName}\";

    /// <summary>
    /// Initializes a new instance of the <see cref="DbUpdaterFromScvFiles"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public DbUpdaterFromScvFiles(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    private IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Update all entities from CSV files.
    /// </summary>
    /// <returns>Result.</returns>
    public string UpdateAll()
    {
        var result = string.Empty;
        //result += FoundPathsMessage;
        var pathsWithDirectories = GetPaths();
        var paths = pathsWithDirectories.Select(path => path[..path.LastIndexOf('|')]).ToList();
        //result += string.Join("\n", paths) + "\n";
        if (paths.Exists(path => !path.Contains('\\')))
        {
            return result;
        }

        result += UpdateEntities<
            FullGpParametersScvModel,
            AddOrUpdateGpParameterFromCsvFileCommand>(
            pathsWithDirectories.Where(path => path[(path.LastIndexOf('|') + 1) ..] == GpParametersDirectoryName)
                .Select(path => path[..path.LastIndexOf('|')])
                .ToList());

        result += ", ";

        result += UpdateEntities<
            TrustedSoftwareScvModel,
            AddOrUpdateTrustedSoftwareFromCsvFileCommand>(
            pathsWithDirectories.Where(path => path[(path.LastIndexOf('|') + 1) ..] == SoftwareDirectoryName)
                .Select(path => path[..path.LastIndexOf('|')])
                .ToList());

        result += ", ";

        result += UpdateEntities<
            TrustedHardwareScvModel,
            AddOrUpdateTrustedHardwareFromCsvFileCommand>(
            pathsWithDirectories.Where(path => path[(path.LastIndexOf('|') + 1) ..] == HardwareDirectoryName)
                .Select(path => path[..path.LastIndexOf('|')])
                .ToList());

        result += SuccessfullyMessage;
        result = result.ToLower();
        result = string.Concat(result[0].ToString().ToUpper(), result.AsSpan(1));

        return result;
    }

    private string UpdateEntities<TQueryResult, TCommand>(List<string> paths)
        where TQueryResult : IScvModel
        where TCommand : CommandBase<List<TQueryResult>>
    {
        var query = ServiceProvider.GetRequiredService<QueryFromCsvFile<TQueryResult>>();
        var queryResult = query.Execute(paths);
        if (!queryResult.IsSuccessful)
        {
            return queryResult.ErrorMessage;
        }

        var command = ServiceProvider.GetRequiredService<TCommand>();
        var commandResult = command.Execute(queryResult.Data);
        if (!commandResult.IsSuccessful)
        {
            return commandResult.ErrorMessage;
        }

        var fileNames = paths.Select(path => path[(path.LastIndexOf('\\') + 1) ..path.LastIndexOf('.')]);
        string fileNamesSeparatedByComma = string.Join(", ", fileNames);

        return fileNamesSeparatedByComma;
    }

    private static void CopyDirectory(string sourcePath, string targetPath)
    {
        foreach (string directoryPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(directoryPath.Replace(sourcePath, targetPath));
        }

        foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(filePath, filePath.Replace(sourcePath, targetPath), true);
        }
    }

    private static List<string> GetPaths()
    {
        try
        {
            var scvFilesPaths = GetScvFilesPaths();

            return scvFilesPaths;
        }
        catch (Exception exception)
        {
            return new List<string> { @"Не удалось получить пути к csv-файлам из-за ошибки." + "\n" + $"{exception}" };
        }
    }

    private static List<string> GetScvFilesPaths()
    {
        if (!Directory.Exists(UserDbDirectoryPath))
        {
            if (!Directory.Exists(SourceDbDirectoryPath))
            {
                return new List<string> { @"Не удалось получить пути к csv-файлам из-за отсутствия файлов." };
            }

            if (Directory.Exists(SourceDbDirectoryPath))
            {
                CopyDirectory(SourceDbDirectoryPath, UserDbDirectoryPath);
            }
        }

        var scvFilesPaths = new List<string>();
        var directoryInfo = new DirectoryInfo(UserDbDirectoryPath);
        var csvDirectoriesInfos = directoryInfo.EnumerateDirectories();
        foreach (var csvDirectoryInfo in csvDirectoriesInfos)
        {
            var filesInfos = csvDirectoryInfo.EnumerateFiles();
            scvFilesPaths.AddRange(filesInfos.Select(filesInfo => filesInfo.FullName + $"|{filesInfo.Directory?.Name}"));
        }

        return scvFilesPaths;
    }
}