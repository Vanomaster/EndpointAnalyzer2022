namespace Queries.NonDatabase;

/// <summary>
/// Model for RegistryParameterValueQuery.
/// </summary>
public class RegistryParameterQueryModel
{
    /// <summary>
    /// The full registry path of the key, beginning with a valid registry root, such as "HKEY_CURRENT_USER".
    /// </summary>
    public string KeyName { get; set; }

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string ParameterName { get; set; }
}