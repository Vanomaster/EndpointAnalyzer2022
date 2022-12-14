using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.Base;
using Queries.NonDatabase;

namespace Queries.Database;

/// <inheritdoc />
public class TrustedSoftwareQuery : QueryBase<List<Guid>?, List<SimpleSoftware>>
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
    protected override QueryResult<List<SimpleSoftware>> ExecuteCore(List<Guid>? trustedSoftwareIds)
    {
        List<SimpleSoftware> trustedSimpleSoftware = new ();
        var entitiesToFetch = Context.TrustedSoftware.AsNoTracking();
        if (trustedSoftwareIds is not null)
        {
            entitiesToFetch = entitiesToFetch.Where(entity => trustedSoftwareIds.Contains(entity.Id));
        }

        var trustedSoftware = entitiesToFetch.ToList();
        foreach (var software in trustedSoftware)
        {
            var softwareVersion = Version.Parse(software.Version);
            SimpleSoftware simpleSoftware = new () { Name = software.Name, Version = softwareVersion };
            trustedSimpleSoftware.Add(simpleSoftware);
        }

        return GetSuccessfulResult(trustedSimpleSoftware);
    }
}