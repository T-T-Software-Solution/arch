namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with asynchronous query operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IAsyncQueryRepository<TEntity, TKey> : IQueryRepository<TEntity, TKey>
{
    /// <summary>
    /// Get entities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entities</returns>
    IAsyncEnumerable<TEntity> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get entities.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Entities</returns>
    IAsyncEnumerable<TEntity> GetAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}