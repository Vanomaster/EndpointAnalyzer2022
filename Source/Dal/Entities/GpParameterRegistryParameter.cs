namespace Dal.Entities;

public class GpParameterRegistryParameter : IEntity
{
    public Guid Id { get; set; }

    public Guid GpParameterId { get; set; }

    public string KeyName { get; set; } = null!;

    public string ParameterName { get; set; } = null!;

    public virtual GpParameter GpParameter { get; set; } = null!;
}