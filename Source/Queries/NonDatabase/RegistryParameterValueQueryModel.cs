namespace Queries.NonDatabase;

/// <summary>
/// Model for RegistryParameterValueQuery.
/// </summary>
public class RegistryParameterValueQueryModel
{
    /// <summary>
    /// The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
    /// </summary>
    public string KeyName { get; set; }

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string? ValueName { get; set; }
}