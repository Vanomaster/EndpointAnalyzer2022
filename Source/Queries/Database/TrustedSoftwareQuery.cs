using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.Base;

namespace Queries.Database;

/// <inheritdoc />
public class TrustedSoftwareQuery : QueryBase<List<Guid>?, List<TrustedSoftware>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrustedSoftwareQuery"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public TrustedSoftwareQuery(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override QueryResult<List<TrustedSoftware>> ExecuteCore(List<Guid>? trustedSoftwareIds)
    {
        var entitiesToFetch = Context.TrustedSoftware.AsNoTracking();
        if (trustedSoftwareIds is not null)
        {
            entitiesToFetch = entitiesToFetch.Where(entity => trustedSoftwareIds.Contains(entity.Id));
        }

        var trustedSoftware = entitiesToFetch.ToList();

        return GetSuccessfulResult(trustedSoftware);
    }
}