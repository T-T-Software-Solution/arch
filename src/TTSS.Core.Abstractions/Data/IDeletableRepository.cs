namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with delete operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public interface IDeletableRepository<TEntity, TKey> : IRepositoryBase
{
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
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task<bool> DeleteManyAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
}