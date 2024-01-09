using System.Collections;

namespace TTSS.Core.Data;

internal sealed class InMemoryQueryResult<TEntity> : IQueryResult<TEntity>
{
    #region Fields

    private readonly IEnumerable<TEntity> _entities;

    #endregion

    #region Properties

    public long TotalCount => _entities.Count();

    #endregion

    #region Constructors

    public InMemoryQueryResult(IEnumerable<TEntity> entities)
        => _entities = entities;

    #endregion

    #region Methods

    public Task<IEnumerable<TEntity>> GetAsync()
        => Task.FromResult(_entities);

    public IPagingRepositoryResult<TEntity> ToPaging(bool totalCount = false, int pageSize = 0)
        => new InMemoryPagingResult<TEntity>(_entities, totalCount, pageSize);

    public IEnumerator<TEntity> GetEnumerator()
        => _entities.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _entities.GetEnumerator();

    #endregion
}