using Commands.Base;
using Dal;
using Dal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Commands;

/// <inheritdoc />
public class DeleteEntityCommand : CommandBase<IEnumerable<IEntity>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteEntityCommand"/> class.
    /// </summary>
    /// <param name="contextFactory">Context factory.</param>
    public DeleteEntityCommand(IDbContextFactory<Context> contextFactory)
        : base(contextFactory)
    {
    }

    /// <inheritdoc/>
    protected override CommandResult ExecuteCore(IEnumerable<IEntity> entities)
    {
        Context.RemoveRange(entities);
        Context.SaveChanges();

        return GetSuccessfulResult();
    }
}