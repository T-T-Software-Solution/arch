using System.Linq.Expressions;
using TTSS.Core.Data;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.Sql;

internal sealed class SqlPagingRepository<TEntity> : IPagingRepository<TEntity>
{
    #region Fields

    private readonly int _pageSize;
    private readonly int _totalCount;
    private IQueryable<TEntity> _findResult;
    private readonly IMappingStrategy _mappingStrategy;

    #endregion

    #region Constructors

    public SqlPagingRepository(IQueryable<TEntity> findResult, IMappingStrategy mappingStrategy, bool totalCount = false, int pageSize = 0)
    {
        _findResult = findResult;
        _mappingStrategy = mappingStrategy;
        _pageSize = pageSize;
        _totalCount = totalCount ? _findResult.Count() : 0;
    }

    #endregion

    #region Methods

    public PagingSet<TEntity> GetPage(int pageNo)
        => new(GetPageDataInternal(pageNo), _pageSize, pageNo, () => _totalCount, _mappingStrategy);

    public Task<IEnumerable<TEntity>> GetDataAsync(int pageNo)
        => GetPageDataInternal(pageNo);

    public IPagingRepository<TEntity> OrderBy(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.OrderBy(keySelector);
        return this;
    }

    public IPagingRepository<TEntity> OrderByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.OrderByDescending(keySelector);
        return this;
    }

    public IPagingRepository<TEntity> ThenBy(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.Order().ThenBy(keySelector);
        return this;
    }

    public IPagingRepository<TEntity> ThenByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.Order().ThenByDescending(keySelector);
        return this;
    }

    private Task<IEnumerable<TEntity>> GetPageDataInternal(int pageNo)
        => _findResult.Skip(pageNo * _pageSize).Take(_pageSize).GetAsync<TEntity>();

    #endregion
}