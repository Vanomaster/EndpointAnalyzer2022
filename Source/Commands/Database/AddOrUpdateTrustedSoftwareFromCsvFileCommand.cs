using Commands.Base;
using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.NonDatabase;

namespace Commands;

/// <inheritdoc />
public class AddOrUpdateTrustedSoftwareFromCsvFileCommand : CommandBase<List<TrustedSoftwareScvModel>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddOrUpdateEntityCommand"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public AddOrUpdateTrustedSoftwareFromCsvFileCommand(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override CommandResult ExecuteCore(List<TrustedSoftwareScvModel> trustedSoftwareScvModels)
    {
        var trustedSoftware = trustedSoftwareScvModels
            .Select(scvModel => new TrustedSoftware { Name = scvModel.Name, Version = scvModel.Version })
            .ToList();

        Context.UpdateRange(trustedSoftware);
        Context.SaveChanges();

        return GetSuccessfulResult();
    }
}