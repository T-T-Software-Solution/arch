using System.Linq.Expressions;
using TTSS.Core.Data;
using TTSS.Core.Models;

namespace TTSS.Core.Services;

internal sealed class PagingService
{
    public static async Task<IPagingResponse<TViewModel>> GetPagingAsync<TEntity, TKey, TViewModel>(IRepository<TEntity, TKey> repository,
        int pageNo,
        int pageSize,
        IMappingStrategy mappingStrategy,
        Expression<Func<TEntity, bool>>? filter = default,
        Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>>? decorate = default,
        CancellationToken cancellationToken = default)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
    {
        filter ??= (_ => true);
        var paging = repository.Get(filter, cancellationToken)
            .ToPaging(true, pageSize);

        const int Offset = 1;
        pageNo -= Offset;
        if (pageNo < 0) pageNo = 0;

        var page = await (decorate is null ? paging : decorate(paging))
            .GetPage(pageNo)
        .ToPagingDataAsync();

        var itemNumber = pageNo * pageSize;
        var vms = page.Result.Select(mappingStrategy.Map<TEntity, TViewModel>).ToList();
        var orderableQry = vms
            .Where(it => it is IHaveOrderNumber)
            .Cast<IHaveOrderNumber>();
        foreach (var item in orderableQry)
        {
            item.OrderNo = ++itemNumber;
        }

        var pageCount = page.PageCount == default ? Offset : page.PageCount;
        var nextPage = page.NextPage == default ? default : page.NextPage + Offset;
        var previousPage = page.PreviousPage == default && page.CurrentPage != Offset ? default : page.PreviousPage + Offset;
        return new PagingResponse<TViewModel>
        {
            PageCount = pageCount,
            CurrentPage = page.CurrentPage + Offset,
            NextPage = nextPage,
            PreviousPage = previousPage,
            HasNextPage = page.HasNextPage,
            HasPreviousPage = page.HasPreviousPage,
            TotalCount = page.TotalCount,
            Data = vms,
        };
    }
}