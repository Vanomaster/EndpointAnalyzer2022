using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.Base;
using Queries.NonDatabase;

namespace Queries.Database;

/// <inheritdoc />
public class GpParametersValuesRecommendationsQuery : QueryBase<List<SimpleGpParameter>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrustedHardwareQuery"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public GpParametersValuesRecommendationsQuery(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override QueryResult<List<SimpleGpParameter>> ExecuteCore()
    {
        var entitiesToFetch = Context.GpParametersRecommendations.AsNoTracking();

        var gpParametersValueRecommendation = entitiesToFetch
            .Select(entity => new SimpleGpParameter
            {
                RegistryParameterName = entity.GpParameter.GpParameterRegistryParameters
                    .FirstOrDefault().ParameterName,
                Value = entity.Value,
                Direction = entity.Direction,
            })
            .ToList();

        return GetSuccessfulResult(gpParametersValueRecommendation);
    }
}