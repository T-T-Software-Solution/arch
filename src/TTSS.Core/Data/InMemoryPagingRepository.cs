using System.Linq.Expressions;
using TTSS.Core.Services;

namespace TTSS.Core.Data;

internal sealed class InMemoryPagingRepository<TEntity> : IPagingRepository<TEntity>
{
    #region Fields

    private readonly int _pageSize;
    private readonly int _totalCount;
    private IEnumerable<TEntity> _entities;
    private readonly IMappingStrategy _mappingStrategy;

    #endregion

    #region Constructors

    internal InMemoryPagingRepository(IEnumerable<TEntity> entities, IMappingStrategy mappingStrategy, bool totalCount = false, int pageSize = 0)
    {
        _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        _mappingStrategy = mappingStrategy;
        _pageSize = pageSize;
        _totalCount = totalCount ? entities.Count() : 0;
    }

    #endregion

    #region Methods

    public PagingSet<TEntity> GetPage(int pageNo)
        => new(Task.FromResult(GetPageDataInternal(pageNo)), _pageSize, pageNo, () => _totalCount, _mappingStrategy);

    public Task<IEnumerable<TEntity>> GetDataAsync(int pageNo)
        => Task.FromResult(GetPageDataInternal(pageNo));

    public IPagingRepository<TEntity> OrderBy(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities.OrderBy(keySelector.Compile());
        return this;
    }

    public IPagingRepository<TEntity> OrderByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities.OrderByDescending(keySelector.Compile());
        return this;
    }

    public IPagingRepository<TEntity> ThenBy(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities is IOrderedEnumerable<TEntity> orderedData ? orderedData.ThenBy(keySelector.Compile()) : _entities.OrderBy(keySelector.Compile());
        return this;
    }

    public IPagingRepository<TEntity> ThenByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities is IOrderedEnumerable<TEntity> orderedData ? orderedData.ThenByDescending(keySelector.Compile()) : _entities.OrderByDescending(keySelector.Compile());
        return this;
    }

    private IEnumerable<TEntity> GetPageDataInternal(int pageNo)
    {
        const int MinPageNo = 0;
        pageNo = pageNo <= MinPageNo ? MinPageNo : --pageNo;
        return _entities
            .Skip(pageNo * _pageSize)
            .Take(_pageSize);
    }

    #endregion
}