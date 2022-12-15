using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Query for getting data from CSV file.
/// </summary>
/// <typeparam name="TResult">Type of query result.</typeparam>
public class QueryFromCsvFile<TResult> : NonDbQueryBase<List<string>, List<TResult>>
{
    private readonly CsvConfiguration configuration = new (CultureInfo.CurrentCulture)
    {
        HasHeaderRecord = true,
        Delimiter = "|",
        BadDataFound = null,
    };

    /// <inheritdoc/>
    protected override QueryResult<List<TResult>> ExecuteCore(List<string> paths)
    {
        var records = new List<TResult>();
        foreach (string path in paths)
        {
            using var streamReader = File.OpenText(path);
            using var csvReader = new CsvReader(streamReader, configuration);
            while (csvReader.Read())
            {
                var record = csvReader.GetRecord<TResult>();
                if (record is null)
                {
                    return GetFailedResult(@"Record is empty.");
                }

                records.Add(record);
            }
        }

        return GetSuccessfulResult(records);
    }
}