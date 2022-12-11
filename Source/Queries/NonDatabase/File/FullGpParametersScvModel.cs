namespace Queries.NonDatabase;

/// <summary>
/// Result of QueryFromCsvFile.
/// </summary>
public class FullGpParametersScvModel : IScvModel
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Value { get; set; } = null!;

    public bool? Direction { get; set; }

    public string? Rationale { get; set; }

    public string? Impact { get; set; }

    public string KeyName { get; set; } = null!;

    public string ParameterName { get; set; } = null!;
}