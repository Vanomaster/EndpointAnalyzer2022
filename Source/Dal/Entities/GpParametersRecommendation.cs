namespace Dal.Entities;

public class GpParametersRecommendation : IEntity
{
    public Guid Id { get; set; }

    public Guid GpParameterId { get; set; }

    public string Value { get; set; } = null!;

    public bool? Direction { get; set; }

    public string? Rationale { get; set; }

    public string? Impact { get; set; }

    public virtual GpParameter GpParameter { get; set; } = null!;

    public virtual IEnumerable<GpParametersRecommendationsBenchmark> GpParametersRecommendationsBenchmarks { get; }
        = new List<GpParametersRecommendationsBenchmark>();
}