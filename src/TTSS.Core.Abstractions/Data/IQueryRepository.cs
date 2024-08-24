using System.Linq.Expressions;
using TTSS.Core.Models;

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
    /// Get paged data.
    /// </summary>
    /// <typeparam name="TViewModel">Output view model type</typeparam>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paging result</returns>
    Task<IPagingResponse<TViewModel>> GetPagingAsync<TViewModel>(
        int pageNo,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged data.
    /// </summary>
    /// <typeparam name="TViewModel">Output view model type</typeparam>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paging result</returns>
    Task<IPagingResponse<TViewModel>> GetPagingAsync<TViewModel>(
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged data.
    /// </summary>
    /// <typeparam name="TViewModel">Output view model type</typeparam>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <param name="decorate">Decorate function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paging result</returns>
    Task<IPagingResponse<TViewModel>> GetPagingAsync<TViewModel>(
        int pageNo,
        int pageSize,
        Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>> decorate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged data.
    /// </summary>
    /// <typeparam name="TViewModel">Output view model type</typeparam>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// /// <param name="filter">Entity filter</param>
    /// <param name="decorate">Decorate function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paging result</returns>
    Task<IPagingResponse<TViewModel>> GetPagingAsync<TViewModel>(
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>> filter,
        Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>> decorate,
        CancellationToken cancellationToken = default);
}