using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.Base;

namespace Queries.Database;

/// <inheritdoc />
public class GpParameterRegistryParametersQueryByGpParameterName
    : QueryBase<List<string>?, List<GpParameterRegistryParameter>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GpParameterRegistryParametersQueryByGpParameterName"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public GpParameterRegistryParametersQueryByGpParameterName(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override QueryResult<List<GpParameterRegistryParameter>> ExecuteCore(List<string>? gpParameterNames)
    {
        var entitiesToFetch = Context.GpParameterRegistryParameters.AsNoTracking();
        if (gpParameterNames is not null)
        {
            entitiesToFetch = entitiesToFetch.Where(entity => gpParameterNames.Contains(entity.GpParameter.Name));
        }

        var gpParameterRegistryParameters = entitiesToFetch.ToList();

        return GetSuccessfulResult(gpParameterRegistryParameters);
    }
}