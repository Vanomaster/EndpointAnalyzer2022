namespace Queries.NonDatabase;

public class RegistryHardware
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime? LastHardwareDataModifiedDateTime { get; set; }

    public string Location { get; set; }
}