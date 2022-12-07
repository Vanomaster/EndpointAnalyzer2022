namespace Dal.Configuration;

/// <summary>
/// Configuration helper.
/// </summary>
public interface IConfigurationHelper
{
    /// <summary>
    /// Connection string to main DB.
    /// </summary>
    public string? MainConnectionString { get; }
}