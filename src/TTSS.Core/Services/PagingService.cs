using System.Linq.Expressions;
using TTSS.Core.Data;

namespace TTSS.Core.Services;

internal sealed class PagingService
{
    private const int Offset = 1;
    private const int MinimumPageNo = 0;

    public static PagingResult<TEntity> GetPaging<TEntity, TKey>(IRepository<TEntity, TKey> repository,
        int pageNo,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = default,
        Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>>? decorate = default)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        pageNo -= Offset;
        if (pageNo < MinimumPageNo)
        {
            pageNo = MinimumPageNo;
        }

        var paging = repository.Get(filter ?? (_ => true)).ToPaging(true, pageSize);
        var pagingRepository = decorate is null ? paging : decorate(paging);
        return pagingRepository.GetPage(pageNo);
    }
}