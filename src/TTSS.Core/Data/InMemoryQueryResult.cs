﻿using System.Collections;
using TTSS.Core.Services;

namespace TTSS.Core.Data;

internal sealed class InMemoryQueryResult<TEntity>(IEnumerable<TEntity> entities, IMappingStrategy mappingStrategy) : IQueryResult<TEntity>
{
    #region Fields

    private readonly IEnumerable<TEntity> _entities = entities;

    #endregion

    #region Properties

    public long TotalCount => _entities.Count();

    #endregion

    #region Methods

    public Task<IEnumerable<TEntity>> GetAsync()
        => Task.FromResult(_entities);

    public IPagingRepository<TEntity> ToPaging(bool totalCount = false, int pageSize = 0)
        => new InMemoryPagingRepository<TEntity>(_entities, mappingStrategy, totalCount, pageSize);

    public IEnumerator<TEntity> GetEnumerator()
        => _entities.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _entities.GetEnumerator();

    #endregion
}