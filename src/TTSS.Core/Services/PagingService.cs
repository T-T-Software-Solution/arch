using System.Linq.Expressions;
using TTSS.Core.Data;

namespace TTSS.Core.Services;

internal sealed class PagingService
{
    private const int Offset = 1;
    private const int MinimumPageNo = 0;

    public static PagingSet<TEntity> GetPaging<TEntity, TKey>(IRepository<TEntity, TKey> srcRepository,
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = default,
        Action<IPagingRepository<TEntity>>? decorate = default)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        pageNo -= Offset;
        if (pageNo < MinimumPageNo)
        {
            pageNo = MinimumPageNo;
        }

        var pagingRepository = srcRepository.Get(filter ?? (_ => true)).ToPaging(true, pageSize);
        decorate?.Invoke(pagingRepository);
        return pagingRepository.GetPage(pageNo);
    }
}