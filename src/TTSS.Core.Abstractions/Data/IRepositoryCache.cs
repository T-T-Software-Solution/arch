namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with cache.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IRepositoryCache<TEntity, TKey> : IRepositoryBase,
    IDisposable
    where TEntity : class
    where TKey : notnull
{
    /// <summary>
    /// Get an entity by id.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity</returns>
    Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filterKeys">Filter keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    Task<IEnumerable<TEntity>> GetAsync(IEnumerable<TKey> filterKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> SetAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete entities.
    /// </summary>
    /// <param name="filterKeys">Filter keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> DeleteManyAsync(IEnumerable<TKey> filterKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increase the value of the counter for the repository in the cache.
    /// </summary>
    /// <param name="incValue">Increment value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The incremented value</returns>
    Task<long> IncrementCounterAsync(long incValue = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decrease the value of the counter for the repository in the cache.
    /// </summary>
    /// <param name="decValue">Decrement value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The decremented value</returns>
    Task<long> DecrementCounterAsync(long decValue = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset the value of the counter for the repository in the cache.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> ResetCounter(CancellationToken cancellationToken = default);
}

/// <summary>
/// Contract for repository with cache and string primary key.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRepositoryCache<TEntity> : IRepositoryCache<TEntity, string>
    where TEntity : class
{
}