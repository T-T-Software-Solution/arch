namespace TTSS.Core.Data;

/// <summary>
/// Contract for query result.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IQueryResult<TEntity> : IEnumerable<TEntity>
{
    /// <summary>
    /// Total entity count.
    /// </summary>
    long TotalCount { get; }

    /// <summary>
    /// Get data.
    /// </summary>
    /// <returns>The entities</returns>
    Task<IEnumerable<TEntity>> GetAsync();

    /// <summary>
    /// Convert to paging result.
    /// </summary>
    /// <param name="totalCount">Force calculation of total entities or not</param>
    /// <param name="pageSize">Total entities per paging result</param>
    /// <returns>The paging result</returns>
    IPagingRepositoryResult<TEntity> ToPaging(bool totalCount = false, int pageSize = 0);
}