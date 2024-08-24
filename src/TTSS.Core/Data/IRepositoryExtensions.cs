using AutoMapper;
using System.Linq.Expressions;
using TTSS.Core.Models;

namespace TTSS.Core.Data;

/// <summary>
/// Entity extensions.
/// </summary>
public static class IRepositoryExtensions
{
    public static async Task<IPagingResponse<TViewModel>> GetPagingAsync<TEntity, TViewModel>(this IRepository<TEntity> target,
        Expression<Func<TEntity, bool>> filter,
        Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>> decorate,
        IPagingRequest request,
        IMapper mapper,
        CancellationToken cancellationToken = default)
        where TEntity : class, IDbModel<string>
    {
        var paging = target.Get(filter, cancellationToken)
            .ToPaging(true, request.PageSize);

        const int Offset = 1;
        var pageNo = request.PageNo - Offset;
        var page = await (decorate is null ? paging : decorate(paging))
            .GetPage(pageNo)
            .ToPagingDataAsync();

        var itemNumber = pageNo * request.PageSize;
        var vms = page.Result.Select(mapper.Map<TViewModel>).ToList();
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