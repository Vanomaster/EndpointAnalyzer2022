using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.Base;
using Queries.NonDatabase;

namespace Queries.Database;

/// <inheritdoc />
public class GpParametersRationalesRecommendationsQuery : QueryBase<List<string>, List<SimpleGpParameterRationale>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrustedHardwareQuery"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public GpParametersRationalesRecommendationsQuery(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override QueryResult<List<SimpleGpParameterRationale>> ExecuteCore(List<string>? parametersNames)
    {
        var entitiesToFetch = Context.GpParametersRecommendations.AsNoTracking();

        if (parametersNames is not null)
        {
            entitiesToFetch = entitiesToFetch
                .Where(entity => parametersNames
                    .Contains(entity.GpParameter.GpParameterRegistryParameters
                        .FirstOrDefault().ParameterName));
        }

        var gpParametersRationalesRecommendation = entitiesToFetch
            .Select(entity => new SimpleGpParameterRationale
            {
                Name = entity.GpParameter.Name,
                RegistryParemeterName = entity.GpParameter.GpParameterRegistryParameters
                    .FirstOrDefault().ParameterName,
                Value = entity.Value,
                Direction = entity.Direction,
                Rationale = entity.Rationale,
                Impact = entity.Impact,
                Description = entity.GpParameter.Description,
            })
            .ToList();

        return GetSuccessfulResult(gpParametersRationalesRecommendation);
    }
}