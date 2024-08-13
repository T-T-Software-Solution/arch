using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// SQL database interceptor base.
/// </summary>
public abstract class SqlSaveChangesInterceptorBase : SaveChangesInterceptor, IDbInterceptor
{
    #region Methods

    /// <summary>
    /// Called at the start of entity saving.
    /// </summary>
    /// <param name="eventData">Contextual information about the DbContext being used</param>
    /// <param name="result">Represents the current result if one exists</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>If HasResult is false, the EF will continue as normal. If HasResult is true, then EF will suppress the operation it was about to perform and use Result instead. An implementation of this method for any interceptor that is not attempting to change the result is to return the result value passed in.</returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var entries = eventData.Context?.ChangeTracker?.Entries()
            .Where(it => it is not null && it.Entity is IDbModel)
            .Select(it => new { Entry = it, Entity = (IDbModel)it.Entity, }) ?? [];
        foreach (var item in entries)
        {
            switch (item.Entry.State)
            {
                case EntityState.Added:
                    {
                        await OnCreateAsync(item.Entity, cancellationToken);
                        break;
                    }
                case EntityState.Modified:
                    {
                        var entityType = item.Entity.GetType();
                        var changedProperties = item.Entry.Properties
                            .Where(it => it.IsModified)
                            .Select(it => new PropertyUpdateInfo
                            {
                                ColumnName = it.Metadata.Name,
                                NewValue = it.CurrentValue?.ToString(),
                                OriginalValue = it.OriginalValue?.ToString(),
                                Remark = entityType
                                    .GetProperty(it.Metadata.Name)
                                    ?.GetCustomAttribute<CommentAttribute>()
                                    ?.Comment,
                            })
                            .Where(it => it.OriginalValue != it.NewValue)
                            .Distinct()
                            .ToList();
                        await OnUpdateAsync(item.Entity, changedProperties, cancellationToken);
                        break;
                    }
                case EntityState.Deleted:
                    {
                        await OnDeleteAsync(item.Entity, cancellationToken);
                        break;
                    }
                default: break;
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// On create entity.
    /// </summary>
    /// <param name="entity">Target entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if the operation is successful; otherwise, false.</returns>
    /// <remarks>The false value will stop the operation.</remarks>
    protected abstract Task OnCreateAsync(IDbModel entity, CancellationToken cancellationToken);

    /// <summary>
    /// On update entity.
    /// </summary>
    /// <param name="entity">Target entity</param>
    /// <param name="infos">Update information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if the operation is successful; otherwise, false.</returns>
    /// <remarks>The false value will stop the operation.</remarks>
    protected abstract Task OnUpdateAsync(IDbModel entity, IEnumerable<PropertyUpdateInfo> infos, CancellationToken cancellationToken);

    /// <summary>
    /// On delete entity.
    /// </summary>
    /// <param name="entity">Target entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if the operation is successful; otherwise, false.</returns>
    /// <remarks>The false value will stop the operation.</remarks>
    protected abstract Task OnDeleteAsync(IDbModel entity, CancellationToken cancellationToken);

    #endregion
}