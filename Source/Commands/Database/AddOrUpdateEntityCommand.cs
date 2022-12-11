using Commands.Base;
using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Commands;

/// <inheritdoc />
public class AddOrUpdateEntityCommand : CommandBase<IEnumerable<IEntity>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddOrUpdateEntityCommand"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public AddOrUpdateEntityCommand(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override CommandResult ExecuteCore(IEnumerable<IEntity> entities)
    {
        Context.UpdateRange(entities);
        Context.SaveChanges();

        return GetSuccessfulResult();
    }
}