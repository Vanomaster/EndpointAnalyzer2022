using Dal.Configuration;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dal;

/// <inheritdoc />
public class Context : DbContext
{
    /// <summary>
    /// Only for migrations. Initializes a new instance of the <see cref="Context"/> class.
    /// </summary>
    public Context()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Context"/> class.
    /// </summary>
    /// <param name="options">Context options.</param>
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    /// <summary>
    /// Benchmarks.
    /// </summary>
    public DbSet<Benchmark> Benchmarks { get; set; } = null!;

    /// <summary>
    /// Group policy parameters.
    /// </summary>
    public DbSet<GpParameter> GpParameters { get; set; } = null!;

    /// <summary>
    /// Group policy parameter registry parameters.
    /// </summary>
    public DbSet<GpParameterRegistryParameter> GpParameterRegistryParameters { get; set; } = null!;

    /// <summary>
    /// Group policy parameters recommendations.
    /// </summary>
    public DbSet<GpParametersRecommendation> GpParametersRecommendations { get; set; } = null!;

    /// <summary>
    /// Group policy parameters recommendations benchmarks.
    /// </summary>
    public DbSet<GpParametersRecommendationsBenchmark> GpParametersRecommendationsBenchmarks { get; set; } = null!;

    /// <summary>
    /// Trusted hardware.
    /// </summary>
    public DbSet<TrustedHardware> TrustedHardware { get; set; } = null!;

    /// <summary>
    /// Trusted hardware benchmarks.
    /// </summary>
    public DbSet<TrustedHardwareBenchmark> TrustedHardwareBenchmarks { get; set; } = null!;

    /// <summary>
    /// Trusted software.
    /// </summary>
    public DbSet<TrustedSoftware> TrustedSoftware { get; set; } = null!;

    /// <summary>
    /// Trusted software benchmarks.
    /// </summary>
    public DbSet<TrustedSoftwareBenchmark> TrustedSoftwareBenchmarks { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CS_AS");

        modelBuilder.Entity<Benchmark>(entity =>
        {
            entity.HasIndex(e => e.Name, "AK_Benchmarks_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.GpParametersRecommendationsBenchmark).WithMany(p => p.Benchmarks)
                .HasForeignKey(d => d.GpParametersRecommendationsBenchmarkId)
                .HasConstraintName("FK_Benchmarks_GpParametersRecommendationsBenchmarks");

            entity.HasOne(d => d.TrustedHardwareBenchmark).WithMany(p => p.Benchmarks)
                .HasForeignKey(d => d.TrustedHardwareBenchmarkId)
                .HasConstraintName("FK_Benchmarks_TrustedHardwareBenchmarks");

            entity.HasOne(d => d.TrustedSoftwareBenchmark).WithMany(p => p.Benchmarks)
                .HasForeignKey(d => d.TrustedSoftwareBenchmarkId)
                .HasConstraintName("FK_Benchmarks_TrustedSoftwareBenchmarks");
        });

        modelBuilder.Entity<GpParameter>(entity =>
        {
            entity.HasIndex(e => e.Name, "AK_GpParameters_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<GpParameterRegistryParameter>(entity =>
        {
            entity.HasIndex(
                e => new { e.KeyName, e.ParameterName },
                "AK_GpParameterRegistryParameters_KeyName_ParameterName")
                .IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.KeyName).HasMaxLength(350);
            entity.Property(e => e.ParameterName).HasMaxLength(100);

            entity.HasOne(d => d.GpParameter).WithMany(p => p.GpParameterRegistryParameters)
                .HasForeignKey(d => d.GpParameterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GpParameterRegistryParameters_GpParameters");
        });

        modelBuilder.Entity<GpParametersRecommendation>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Value).HasMaxLength(500);

            entity.HasOne(d => d.GpParameter).WithMany(p => p.GpParametersRecommendations)
                .HasForeignKey(d => d.GpParameterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GpParametersRecommendations_GpParameters");
        });

        modelBuilder.Entity<GpParametersRecommendationsBenchmark>(entity =>
        {
            entity.HasIndex(e => e.Name, "AK_GpParametersRecommendationsBenchmarks_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasMany(d => d.GpParametersRecommendations).WithMany(p => p.GpParametersRecommendationsBenchmarks)
                .UsingEntity<Dictionary<string, object>>(
                    "BenchmarksGpParametersRecommendationsRelation",
                    r => r.HasOne<GpParametersRecommendation>().WithMany()
                        .HasForeignKey("GpParametersRecommendationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BenchmarksGpParametersRecommendationsRelations_GpParametersRecommendations"),
                    l => l.HasOne<GpParametersRecommendationsBenchmark>().WithMany()
                        .HasForeignKey("GpParametersRecommendationsBenchmarkId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BenchmarksGpParametersRecommendationsRelations_GpParametersRecommendationsBenchmarks"),
                    j =>
                    {
                        j.HasKey("GpParametersRecommendationsBenchmarkId", "GpParametersRecommendationId");
                    });
        });

        modelBuilder.Entity<TrustedHardware>(entity =>
        {
            entity.ToTable("TrustedHardware");

            entity.HasIndex(e => e.HardwareId, "AK_TrustedHardware_HardwareId").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.HardwareId).HasMaxLength(350);
        });

        modelBuilder.Entity<TrustedHardwareBenchmark>(entity =>
        {
            entity.HasIndex(e => e.Name, "AK_TrustedHardwareBenchmarks_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasMany(d => d.TrustedHardware).WithMany(p => p.TrustedHardwareBenchmarks)
                .UsingEntity<Dictionary<string, object>>(
                    "BenchmarksTrustedHardwareRelation",
                    r => r.HasOne<TrustedHardware>().WithMany()
                        .HasForeignKey("TrustedHardwareId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BenchmarksTrustedHardwareRelations_TrustedHardware"),
                    l => l.HasOne<TrustedHardwareBenchmark>().WithMany()
                        .HasForeignKey("TrustedHardwareBenchmarkId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BenchmarksTrustedHardwareRelations_TrustedHardwareBenchmarks"),
                    j =>
                    {
                        j.HasKey("TrustedHardwareBenchmarkId", "TrustedHardwareId");
                    });
        });

        modelBuilder.Entity<TrustedSoftware>(entity =>
        {
            entity.ToTable("TrustedSoftware");

            entity.HasIndex(e => new { e.Name, e.Version }, "AK_TrustedSoftware_Name_Version").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(350);
            entity.Property(e => e.Version).HasMaxLength(100);
        });

        modelBuilder.Entity<TrustedSoftwareBenchmark>(entity =>
        {
            entity.HasIndex(e => e.Name, "AK_TrustedSoftwareBenchmarks_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasMany(d => d.TrustedSoftware).WithMany(p => p.TrustedSoftwareBenchmarks)
                .UsingEntity<Dictionary<string, object>>(
                    "BenchmarksTrustedSoftwareRelation",
                    r => r.HasOne<TrustedSoftware>().WithMany()
                        .HasForeignKey("TrustedSoftwareId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BenchmarksTrustedSoftwareRelations_TrustedSoftware"),
                    l => l.HasOne<TrustedSoftwareBenchmark>().WithMany()
                        .HasForeignKey("TrustedSoftwareBenchmarkId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BenchmarksTrustedSoftwareRelations_TrustedSoftwareBenchmarks"),
                    j =>
                    {
                        j.HasKey("TrustedSoftwareBenchmarkId", "TrustedSoftwareId");
                    });
        });
    }

    /// <summary>
    /// Only for migrations.
    /// </summary>
    /// <param name="optionsBuilder">Options builder only for migrations.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationHelper();
        optionsBuilder.UseSqlServer(configuration.MainConnectionString);
    }
}