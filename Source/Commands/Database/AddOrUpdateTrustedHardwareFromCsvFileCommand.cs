using Commands.Base;
using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Queries.NonDatabase;

namespace Commands;

/// <inheritdoc />
public class AddOrUpdateTrustedHardwareFromCsvFileCommand : CommandBase<List<TrustedHardwareScvModel>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddOrUpdateEntityCommand"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public AddOrUpdateTrustedHardwareFromCsvFileCommand(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override CommandResult ExecuteCore(List<TrustedHardwareScvModel> trustedHardwareScvModels)
    {
        var trustedHardware = trustedHardwareScvModels
            .Select(scvModel => new TrustedHardware { HardwareId = scvModel.HardwareId })
            .ToList();

        Context.UpdateRange(trustedHardware);
        Context.SaveChanges();

        return GetSuccessfulResult();
    }
}