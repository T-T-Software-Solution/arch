namespace TTSS.Core.Data;

/// <summary>
/// Extension methods for working with <see cref="IQueryResult{T}"/>.
/// </summary>
public static class QueryResultExtensions
{
    /// <summary>
    /// Convert to asynchronous enumerable.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="result">The entities result</param>
    /// <returns>The asynchronous enumerable</returns>
    public static Task<IEnumerable<TEntity>> GetAsync<TEntity>(this IEnumerable<TEntity> result)
        => result is IQueryResult<TEntity> qresult
        ? qresult.GetAsync()
        : Task.FromResult(result);

    /// <summary>
    /// Convert to paging result.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="result">The entities result</param>
    /// <param name="totalCount">Force calculation of total entities or not</param>
    /// <param name="pageSize">Total entities per paging result</param>
    /// <returns>The paging result</returns>
    /// <exception cref="NotSupportedException">The result parameter must implement <see cref="IQueryResult{T}"/>.</exception>
    public static IPagingRepository<TEntity> ToPaging<TEntity>(this IEnumerable<TEntity> result, bool totalCount = false, int pageSize = 0)
        => result is IQueryResult<TEntity> qresult
        ? qresult.ToPaging(totalCount, pageSize)
        : throw new NotSupportedException("The underlying result is not paging-enabled.");
}