using TTSS.Core.Data;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Contract for audit repository.
/// </summary>
public interface IAuditRepository
{
    /// <summary>
    /// Add audit entity.
    /// </summary>
    /// <param name="entities">The entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAuditEntityAsync(IEnumerable<IAuditEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add audit entity.
    /// </summary>
    /// <param name="entity">The entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAuditEntityAsync(IAuditEntity entity, CancellationToken cancellationToken = default)
        => AddAuditEntityAsync([entity], cancellationToken);
}