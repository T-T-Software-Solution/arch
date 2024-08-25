using System.Linq.Expressions;

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
    IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged set.
    /// </summary>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <returns>Paging set</returns>
    PagingSet<TEntity> GetPaging(
        int pageNo,
        int pageSize);

    /// <summary>
    /// Get paged set.
    /// </summary>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <param name="filter">Entity filter</param>
    /// <returns>Paging set</returns>
    PagingSet<TEntity> GetPaging(
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>> filter);

    /// <summary>
    /// Get paged set.
    /// </summary>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <param name="decorate">Decorate function</param>
    /// <returns>Paging set</returns>
    PagingSet<TEntity> GetPaging(
        int pageNo,
        int pageSize,
        Action<IPagingRepository<TEntity>> decorate);

    /// <summary>
    /// Get paged set.
    /// </summary>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// /// <param name="filter">Entity filter</param>
    /// <param name="decorate">Decorate function</param>
    /// <returns>Paging set</returns>
    PagingSet<TEntity> GetPaging(
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>> filter,
        Action<IPagingRepository<TEntity>> decorate);
}