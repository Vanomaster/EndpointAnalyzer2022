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

        services.AddScoped<RegistryParameterQueryByGpParameterName>();
        services.AddScoped<InstalledSoftwareQuery>();
        services.AddScoped<UpgradableSoftwareQuery>();
        services.AddScoped<RegistryParameterQuery>();
        services.AddScoped<RegistryExistentParameterQuery>();
        services.AddScoped<ParametersQueryFromSecedit>();

        services.AddScoped<QueryFromCsvFile<FullGpParametersScvModel>>();
        services.AddScoped<QueryFromCsvFile<TrustedSoftwareScvModel>>();
        services.AddScoped<QueryFromCsvFile<TrustedHardwareScvModel>>();

        return services;
    }
}