namespace Dal.Entities;

public class TrustedHardware : IEntity
{
    public Guid Id { get; set; }

    public string HardwareId { get; set; } = null!;

    public virtual IEnumerable<TrustedHardwareBenchmark> TrustedHardwareBenchmarks { get; }
        = new List<TrustedHardwareBenchmark>();
}