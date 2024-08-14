﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

        var creationQry = CreateGroup(it => it == EntityState.Added);
        await ExecuteAsync(creationQry, OnCreateAsync);

        var deletionQry = CreateGroup(it => it == EntityState.Deleted);
        await ExecuteAsync(deletionQry, OnDeleteAsync);

        var updateQry = entries
            .Where(it => it.Entry.State == EntityState.Modified)
            .GroupBy(it => it.Entity, selector => selector.Entry.Properties
                .Where(it => it.IsModified)
                .Select(it => ExtractUpdatePropertyInfo(it, selector.Entity))
                .Where(it => it.Value != it.NewValue)
                .Distinct()
                .ToList());
        await ExecuteAsync(updateQry, OnUpdateAsync);


        return await base.SavingChangesAsync(eventData, result, cancellationToken);
        IEnumerable<IGrouping<IDbModel, IList<SqlPropertyInfo>>> CreateGroup(Func<EntityState, bool> filter)
            => entries
                .Where(it => filter(it.Entry.State))
                .GroupBy(it => it.Entity, selector => selector.Entry.Properties
                    .Select(it => ExtractPropertyInfo(it, selector.Entity))
                    .Distinct()
                    .ToList());
        async Task ExecuteAsync<TProperty>(IEnumerable<IGrouping<IDbModel, IList<TProperty>>> qry, Func<IDbModel, IList<TProperty>, CancellationToken, Task> func) where TProperty : SqlPropertyInfo
        {
            foreach (var group in qry)
            {
                foreach (var item in group)
                {
                    await func(group.Key, item, cancellationToken);
                }
            }
        }
        SqlPropertyInfo ExtractPropertyInfo(PropertyEntry entry, IDbModel entity)
            => new()
            {
                ColumnName = entry.Metadata.Name,
                Value = entry.OriginalValue?.ToString(),
                Remark = GetEntityComment(entity, entry.Metadata.Name),
            };
        SqlUpdatePropertyInfo ExtractUpdatePropertyInfo(PropertyEntry entry, IDbModel entity)
            => new()
            {
                ColumnName = entry.Metadata.Name,
                Value = entry.OriginalValue?.ToString(),
                NewValue = entry.CurrentValue?.ToString(),
                Remark = GetEntityComment(entity, entry.Metadata.Name),
            };
        string? GetEntityComment(IDbModel entity, string propertyName)
        {
            var entityType = entity.GetType();
            return entityType
                .GetProperty(propertyName)
                ?.GetCustomAttribute<CommentAttribute>()
                ?.Comment;
        }
    }

    /// <summary>
    /// On create entity.
    /// </summary>
    /// <param name="entity">Target entity</param>
    /// <param name="properties">Entity properties</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if the operation is successful; otherwise, false.</returns>
    /// <remarks>The false value will stop the operation.</remarks>
    protected abstract Task OnCreateAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken);

    /// <summary>
    /// On update entity.
    /// </summary>
    /// <param name="entity">Target entity</param>
    /// <param name="properties">Entity properties</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if the operation is successful; otherwise, false.</returns>
    /// <remarks>The false value will stop the operation.</remarks>
    protected abstract Task OnUpdateAsync(IDbModel entity, IEnumerable<SqlUpdatePropertyInfo> properties, CancellationToken cancellationToken);

    /// <summary>
    /// On delete entity.
    /// </summary>
    /// <param name="entity">Target entity</param>
    /// <param name="properties">Entity properties</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if the operation is successful; otherwise, false.</returns>
    /// <remarks>The false value will stop the operation.</remarks>
    protected abstract Task OnDeleteAsync(IDbModel entity, IEnumerable<SqlPropertyInfo> properties, CancellationToken cancellationToken);

    #endregion
}