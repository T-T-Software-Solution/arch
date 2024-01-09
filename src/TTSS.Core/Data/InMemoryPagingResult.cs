using System.Linq.Expressions;

namespace TTSS.Core.Data;

internal sealed class InMemoryPagingResult<TEntity> : IPagingRepositoryResult<TEntity>
{
    #region Fields

    private readonly int _pageSize;
    private readonly int _totalCount;
    private IEnumerable<TEntity> _entities;

    #endregion

    #region Constructors

    internal InMemoryPagingResult(IEnumerable<TEntity> entities, bool totalCount = false, int pageSize = 0)
    {
        _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        _pageSize = pageSize;
        _totalCount = totalCount ? entities.Count() : 0;
    }

    #endregion

    #region Methods

    public PagingResult<TEntity> GetPage(int pageNo)
        => new(Task.FromResult(GetPageDataInternal(pageNo)), _pageSize, pageNo, () => _totalCount);

    public Task<IEnumerable<TEntity>> GetDataAsync(int pageNo)
        => Task.FromResult(GetPageDataInternal(pageNo));

    public IPagingRepositoryResult<TEntity> OrderBy(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities.OrderBy(keySelector.Compile());
        return this;
    }

    public IPagingRepositoryResult<TEntity> OrderByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities.OrderByDescending(keySelector.Compile());
        return this;
    }

    public IPagingRepositoryResult<TEntity> ThenBy(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities is IOrderedEnumerable<TEntity> orderedData ? orderedData.ThenBy(keySelector.Compile()) : _entities.OrderBy(keySelector.Compile());
        return this;
    }

    public IPagingRepositoryResult<TEntity> ThenByDescending(Expression<Func<TEntity, object>> keySelector)
    {
        _entities = _entities is IOrderedEnumerable<TEntity> orderedData ? orderedData.ThenByDescending(keySelector.Compile()) : _entities.OrderByDescending(keySelector.Compile());
        return this;
    }

    private IEnumerable<TEntity> GetPageDataInternal(int pageNo)
        => _entities.Skip(pageNo * _pageSize).Take(_pageSize);

    #endregion
}