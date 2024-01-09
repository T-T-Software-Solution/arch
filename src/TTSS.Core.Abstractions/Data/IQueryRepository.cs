namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with query operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IQueryRepository<TEntity, TKey> : IRepositoryBase
{
    /// <summary>
    /// Get an entity by id.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity</returns>
    Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    IEnumerable<TEntity> Get(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    IEnumerable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}