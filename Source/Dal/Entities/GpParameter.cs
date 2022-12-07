namespace Dal.Entities;

public class GpParameter : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual IEnumerable<GpParameterRegistryParameter> GpParameterRegistryParameters { get; }
        = new List<GpParameterRegistryParameter>();

    public virtual IEnumerable<GpParametersRecommendation> GpParametersRecommendations { get; }
        = new List<GpParametersRecommendation>();
}