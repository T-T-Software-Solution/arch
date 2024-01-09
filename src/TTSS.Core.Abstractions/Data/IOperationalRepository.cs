namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with operational operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IOperationalRepository<TEntity, TKey> : IRepositoryBase
{
    /// <summary>
    /// Insert an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> UpdateAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default);
}