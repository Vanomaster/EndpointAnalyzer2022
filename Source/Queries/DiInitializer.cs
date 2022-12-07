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
        services.AddScoped<RegistryParameterValueQuery>();

        return services;
    }
}