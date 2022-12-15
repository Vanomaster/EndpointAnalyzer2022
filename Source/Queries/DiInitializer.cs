using Microsoft.Extensions.DependencyInjection;
using Queries.Database;
using Queries.NonDatabase;

namespace Queries;

/// <summary>
/// Registrar of services in services provider.
/// </summary>
public static class DiInitializer
{
    /// <summary>
    /// Register services in services provider.
    /// </summary>
    /// <param name="services">Collection of services.</param>
    /// <returns>Changed collection of services.</returns>
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        services.AddScoped<TrustedHardwareQuery>();
        services.AddScoped<TrustedSoftwareQuery>();
        services.AddScoped<GpParametersValuesRecommendationsQuery>();
        services.AddScoped<GpParametersRationalesRecommendationsQuery>();
        services.AddScoped<GpRegistryParameterQueryByGpParameterName>();

        services.AddScoped<RegistryParameterQuery>();
        services.AddScoped<RegistryExistentParameterQuery>();
        services.AddScoped<RegistryParametersQueryByKeyName>();
        services.AddScoped<RegistryParameterQueryByGpParameterName>();
        services.AddScoped<ParametersQueryFromSecedit>();
        services.AddScoped<InstalledSoftwareQuery>();
        services.AddScoped<HardwareHistoryQuery>();
        services.AddScoped<HardwareQuery>();

        services.AddScoped<QueryFromCsvFile<FullGpParametersScvModel>>();
        services.AddScoped<QueryFromCsvFile<TrustedSoftwareScvModel>>();
        services.AddScoped<QueryFromCsvFile<TrustedHardwareScvModel>>();

        services.AddScoped<UpgradableSoftwareQuery>();

        return services;
    }
}