namespace Dal.Entities;

public class TrustedSoftware : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Version { get; set; } = null!;

    public virtual IEnumerable<TrustedSoftwareBenchmark> TrustedSoftwareBenchmarks { get; }
        = new List<TrustedSoftwareBenchmark>();
}