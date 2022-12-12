namespace Queries.NonDatabase;

public class SimpleGpParameter
{
    public string RegistryParameterName { get; set; }

    /// <summary>
    /// Current value.
    /// </summary>
    public string? Value { get; set; }

    public bool? Direction { get; set; }
}