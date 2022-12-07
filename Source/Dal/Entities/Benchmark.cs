namespace Dal.Entities;

public class Benchmark : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid? GpParametersRecommendationsBenchmarkId { get; set; }

    public Guid? TrustedSoftwareBenchmarkId { get; set; }

    public Guid? TrustedHardwareBenchmarkId { get; set; }

    public virtual GpParametersRecommendationsBenchmark? GpParametersRecommendationsBenchmark { get; set; }

    public virtual TrustedHardwareBenchmark? TrustedHardwareBenchmark { get; set; }

    public virtual TrustedSoftwareBenchmark? TrustedSoftwareBenchmark { get; set; }
}