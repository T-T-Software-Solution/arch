using System.Linq.Expressions;

namespace TTSS.Core.Data;

/// <summary>
/// Paging helper.
/// </summary>
public static class PagingHelper
{
    /// <summary>
    /// Get paged set.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="srcRepository"></param>
    /// <param name="pageNo">Page number</param>
    /// <param name="pageSize">Content size per page</param>
    /// <param name="filter">Entity filter</param>
    /// <param name="decorate">Decorate function</param>
    /// <returns>Paging set</returns>
    public static PagingSet<TEntity> GetPaging<TEntity, TKey>(IRepository<TEntity, TKey> srcRepository,
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = default,
        Action<IPagingRepository<TEntity>>? decorate = default)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        pageNo--;
        const int MinimumPageNo = 0;
        if (pageNo < MinimumPageNo)
        {
            pageNo = MinimumPageNo;
        }

        var pagingRepository = srcRepository.Get(filter ?? (_ => true)).ToPaging(true, pageSize);
        decorate?.Invoke(pagingRepository);
        return pagingRepository.GetPage(pageNo);
    }
}