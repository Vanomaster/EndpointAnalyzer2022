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
    private const string FoundPathsMessage = "Found paths:\n";
    private const string SuccessfullyMessage = " successfully!\n";

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
        string result = FoundPathsMessage;
        var paths = GetPaths();
        result += string.Join("\n", paths.Select(path => path[..path.LastIndexOf('|')])) + "\n";
        result += UpdateEntities<FullGpParametersScvModel, AddOrUpdateGpParameterFromCsvFileCommand>(paths);
        result += UpdateEntities<TrustedSoftwareScvModel, AddOrUpdateTrustedSoftwareFromCsvFileCommand>(paths);
        result += UpdateEntities<TrustedHardwareScvModel, AddOrUpdateTrustedHardwareFromCsvFileCommand>(paths);

        return result;
    }

    private string UpdateEntities<TQueryResult, TCommand>(List<string> paths)
        where TQueryResult : IScvModel
        where TCommand : CommandBase<List<TQueryResult>>
    {
        paths = paths
            .Where(path => typeof(TQueryResult).Name.Contains(path[(path.LastIndexOf('|') + 1) ..]))
            .Select(path => path[..path.LastIndexOf('|')])
            .ToList();

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

        string commandName = typeof(TCommand).Name;
        string actionName = commandName[..commandName.LastIndexOf("Command", StringComparison.Ordinal)];

        return actionName + SuccessfullyMessage;
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
            return new List<string> { $"Failed to get paths to CSV files due to an error.\n{exception}" };
        }
    }

    private static List<string> GetScvFilesPaths()
    {
        var scvFilesPaths = new List<string>();
        var directory = @$"{Directory.GetCurrentDirectory()}\..\..\..\..\..\Db\";
        var directoryInfo = new DirectoryInfo(directory);
        var csvDirectoriesInfos = directoryInfo.EnumerateDirectories();
        foreach (var csvDirectoryInfo in csvDirectoriesInfos)
        {
            var filesInfos = csvDirectoryInfo.EnumerateFiles();
            scvFilesPaths.AddRange(filesInfos.Select(filesInfo => filesInfo.FullName + $"|{filesInfo.Directory?.Name}"));
        }

        return scvFilesPaths;
    }
}