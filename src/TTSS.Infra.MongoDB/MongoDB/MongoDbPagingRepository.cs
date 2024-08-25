using MongoDB.Driver;
using System.Linq.Expressions;
using TTSS.Core.Data;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.MongoDB;

internal sealed class MongoDbPagingRepository<TEntity> : IPagingRepository<TEntity>
{
    #region Fields

    private readonly int _pageSize;
    private readonly int _totalCount;
    private IFindFluent<TEntity, TEntity> _findResult;
    private readonly IMappingStrategy _mappingStrategy;
    private readonly CancellationToken _cancellationToken;

    #endregion

    #region Constructors

    public MongoDbPagingRepository(IFindFluent<TEntity, TEntity> findResult, IMappingStrategy mappingStrategy, CancellationToken cancellationToken, bool totalCount = false, int pageSize = 0)
    {
        _findResult = findResult;
        _mappingStrategy = mappingStrategy;
        _cancellationToken = cancellationToken;
        _pageSize = pageSize;
        _totalCount = totalCount ? (int)_findResult.CountDocuments(cancellationToken) : 0;
    }

    #endregion

    #region Methods

    public PagingSet<TEntity> GetPage(int pageNo)
        => new(GetPageDataInternal(pageNo), _pageSize, pageNo, () => _totalCount, _mappingStrategy);

    public Task<IEnumerable<TEntity>> GetDataAsync(int pageNo)
        => GetPageDataInternal(pageNo);

    public IPagingRepository<TEntity> OrderBy(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.SortBy(keySelector);
        return this;
    }

    public IPagingRepository<TEntity> OrderByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.SortByDescending(keySelector);
        return this;
    }

    public IPagingRepository<TEntity> ThenBy(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.SortBy(keySelector);
        return this;
    }

    public IPagingRepository<TEntity> ThenByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _findResult = _findResult.SortByDescending(keySelector);
        return this;
    }

    private async Task<IEnumerable<TEntity>> GetPageDataInternal(int pageNo)
        => await _findResult
        .Skip(pageNo * _pageSize)
        .Limit(_pageSize)
        .ToListAsync(_cancellationToken);

    #endregion
}