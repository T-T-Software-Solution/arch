namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with upsert operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IUpsertRepository<TEntity, TKey> : IOperationalRepository<TEntity, TKey>
{
    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> UpsertAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default);
}