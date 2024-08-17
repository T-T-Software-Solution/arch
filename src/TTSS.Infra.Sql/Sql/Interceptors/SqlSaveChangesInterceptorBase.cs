using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using TTSS.Core.Data;
using TTSS.Core.Models;
using TTSS.Core.Services;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql.Interceptors;

/// <summary>
/// SQL database interceptor base.
/// </summary>
public abstract class SqlSaveChangesInterceptorBase(IDateTimeService dateTimeService, ICorrelationContext context) : SaveChangesInterceptor
{
    #region Fields

    internal List<IAuditEntity> _auditEntities = [];

    #endregion Fields

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
            .Where(it => it is not null && it.Entity is IDbModel && it.Entity is not IAuditEntity)
            .Select(it => new { Entry = it, Entity = (IDbModel)it.Entity, }) ?? [];

        if (false == entries.Any())
        {
            return await SavingChangesAsync();
        }

        var creationQry = CreateGroup(it => it == EntityState.Added);
        await ExecuteAsync(creationQry, OnCreateAsync);

        var deletionQry = CreateGroup(it => it == EntityState.Deleted);
        await ExecuteAsync(deletionQry, OnDeleteAsync);

        var updateQry = entries
            .Where(it => it.Entry.State == EntityState.Modified)
            .GroupBy(it => it.Entity, selector => selector.Entry.Properties
                .Where(it => it.IsModified)
                .Select(it => ShouldSkipProperty(it) ? null! : ExtractUpdatePropertyInfo(it, selector.Entity))
                .Where(it => it is not null && it.Value != it.NewValue)
                .Distinct()
                .ToList());
        await ExecuteAsync(updateQry, OnUpdateAsync);

        if (eventData.Context is IAuditRepository auditRepo)
        {
            _auditEntities.RemoveAll(it => eventData.Context!.Entry(it).State == EntityState.Unchanged);
            var entriesQry = _auditEntities
                .Where(it => it is ITimeActivityEntity)
                .Select(it => eventData.Context!.Entry(it));
            SqlActivityLogInterceptor.AssignActivityLog(entriesQry, dateTimeService, context);
            await auditRepo.AddAuditEntityAsync(_auditEntities, cancellationToken);
        }

        return await SavingChangesAsync();

        ValueTask<InterceptionResult<int>> SavingChangesAsync()
            => base.SavingChangesAsync(eventData, result, cancellationToken);
        IEnumerable<IGrouping<IDbModel, IList<SqlPropertyInfo>>> CreateGroup(Func<EntityState, bool> filter)
            => entries
                .Where(it => filter(it.Entry.State))
                .GroupBy(it => it.Entity, selector => selector.Entry.Properties
                    .Select(it => ShouldSkipProperty(it) ? null! : ExtractPropertyInfo(it, selector.Entity))
                    .Where(it => it is not null)
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
        bool ShouldSkipProperty(PropertyEntry entry)
            => (entry.EntityEntry.Entity is ITimeActivityEntity
                && entry.Metadata.Name is nameof(ITimeActivityEntity.CreatedDate)
                or nameof(ITimeActivityEntity.LastUpdatedDate)
                or nameof(ITimeActivityEntity.DeletedDate))
                || (entry.EntityEntry.Entity is IUserActivityEntity
                && entry.Metadata.Name is nameof(IUserActivityEntity.CreatedById)
                or nameof(IUserActivityEntity.LastUpdatedById)
                or nameof(IUserActivityEntity.DeletedById));
    }

    /// <summary>
    /// Add audit entity.
    /// </summary>
    /// <param name="entities">Audit entities</param>
    protected virtual void AddAuditEntity(params IAuditEntity[] entities)
        => _auditEntities.AddRange(entities?.Where(it => it is not null) ?? []);

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