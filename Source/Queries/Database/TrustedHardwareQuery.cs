using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.Base;

namespace Queries.Database;

/// <inheritdoc />
public class TrustedHardwareQuery : QueryBase<List<Guid>?, List<TrustedHardware>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrustedHardwareQuery"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public TrustedHardwareQuery(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override QueryResult<List<TrustedHardware>> ExecuteCore(List<Guid>? trustedHardwareIds)
    {
        var entitiesToFetch = Context.TrustedHardware.AsNoTracking();
        if (trustedHardwareIds is not null)
        {
            entitiesToFetch = entitiesToFetch.Where(entity => trustedHardwareIds.Contains(entity.Id));
        }

        var trustedHardware = entitiesToFetch.ToList();

        return GetSuccessfulResult(trustedHardware);
    }
}