namespace TTSS.Core.Data;

/// <summary>
/// Contract for repository with paging operations.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IPagingRepository<TEntity>
{
    /// <summary>
    /// Get paging by page number.
    /// </summary>
    /// <param name="pageNo">Target page number</param>
    /// <returns>Paging result</returns>
    PagingSet<TEntity> GetPage(int pageNo);

    /// <summary>
    /// Get page data by page number.
    /// </summary>
    /// <param name="pageNo">Target page number</param>
    /// <returns>Page data</returns>
    Task<IEnumerable<TEntity>> GetDataAsync(int pageNo);

    /// <summary>
    /// Reorder data by key selector.
    /// </summary>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Paging repository result</returns>
    IPagingRepository<TEntity> OrderBy(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector);

    /// <summary>
    /// Order data by key selector in descending order.
    /// </summary>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Paging repository result</returns>
    IPagingRepository<TEntity> OrderByDescending(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector);

    /// <summary>
    /// Then order data by key selector.
    /// </summary>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Paging repository result</returns>
    IPagingRepository<TEntity> ThenBy(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector);

    /// <summary>
    /// Then order data by key selector in descending order.
    /// </summary>
    /// <param name="keySelector">Key selector</param>
    /// <returns>Paging repository result</returns>
    IPagingRepository<TEntity> ThenByDescending(System.Linq.Expressions.Expression<Func<TEntity, object>> keySelector);
}