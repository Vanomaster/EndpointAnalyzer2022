﻿namespace Dal.Entities;

public class TrustedSoftwareBenchmark : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual IEnumerable<Benchmark> Benchmarks { get; } = new List<Benchmark>();

    public virtual IEnumerable<TrustedSoftware> TrustedSoftware { get; } = new List<TrustedSoftware>();
}