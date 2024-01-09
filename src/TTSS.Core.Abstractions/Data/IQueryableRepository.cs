namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with queryable operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IQueryableRepository<TEntity> : IRepositoryBase
{
    /// <summary>
    /// Convert to queryable.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queryable</returns>
    IQueryable<TEntity> Query(CancellationToken cancellationToken = default);
}