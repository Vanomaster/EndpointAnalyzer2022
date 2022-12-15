namespace Queries.NonDatabase;

public class RegistryParameter
{
    public string KeyName { get; set; }

    public string ParameterName { get; set; }

    public object? Value { get; set; }

    public DateTime? LastWriteDateTime { get; set; }
}