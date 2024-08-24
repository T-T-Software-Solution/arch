using System.Collections.Concurrent;
using System.Linq.Expressions;
using TTSS.Core.Services;

namespace TTSS.Core.Data;

/// <summary>
/// In-memory implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class InMemoryRepository<TEntity, TKey> : IInMemoryRepository<TEntity, TKey>
    where TEntity : class, IDbModel<TKey>
    where TKey : notnull
{
    #region Fields

    private readonly ConcurrentDictionary<TKey, TEntity> _dataDict;

    #endregion

    #region Properties

    /// <summary>
    /// Mapping strategy.
    /// </summary>
    protected IMappingStrategy MappingStrategy { get; }

    /// <summary>
    /// Get key selector.
    /// </summary>
    protected Func<TEntity, TKey> GetKey { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="InMemoryRepository{TEntity, TKey}"/>.
    /// </summary>
    /// <param name="mappingStrategy">The mapping strategy</param>
    /// <param name="idField">The id field selector</param>
    /// <exception cref="ArgumentNullException">The id field selector is required</exception>
    public InMemoryRepository(IMappingStrategy mappingStrategy, Expression<Func<TEntity, TKey>> idField)
    {
        ArgumentNullException.ThrowIfNull(idField);
        _dataDict = new();
        MappingStrategy = mappingStrategy;
        GetKey = idField.Compile();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get an entity by id.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity</returns>
    public Task<TEntity?> GetByIdAsync(TKey key, CancellationToken cancellationToken = default)
        => Task.FromResult(_dataDict.TryGetValue(key, out var data) ? data : default);

    /// <summary>
    /// Get all data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(CancellationToken cancellationToken = default)
        => new InMemoryQueryResult<TEntity>(_dataDict.Values, MappingStrategy);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => new InMemoryQueryResult<TEntity>(_dataDict.Values.Where(filter.Compile()), MappingStrategy);

    PagingResult<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize)
        => PagingService.GetPaging(this, pageNo, pageSize);

    PagingResult<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize, Expression<Func<TEntity, bool>> filter)
        => PagingService.GetPaging(this, pageNo, pageSize, filter);

    PagingResult<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize, Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>> decorate)
        => PagingService.GetPaging(this, pageNo, pageSize, decorate: decorate);

    PagingResult<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize, Expression<Func<TEntity, bool>> filter, Func<IPagingRepositoryResult<TEntity>, IPagingRepositoryResult<TEntity>> decorate)
        => PagingService.GetPaging(this, pageNo, pageSize, filter, decorate);

    /// <summary>
    /// Convert to queryable.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queryable</returns>
    public IQueryable<TEntity> Query(CancellationToken cancellationToken = default)
        => _dataDict.Values.AsQueryable();

    /// <summary>
    /// Insert an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpsertAsync(GetKey(entity), entity, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpdateAsync(entity.Id, entity, cancellationToken);

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpsertAsync(entity.Id, entity, cancellationToken);

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpsertAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default)
    {
        if (entity == null) return Task.FromResult(false);
        _dataDict[key] = entity;
        return Task.FromResult(true);
    }

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpdateAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default)
        => _dataDict.ContainsKey(key) ? UpsertAsync(GetKey(entity), entity, cancellationToken) : Task.FromResult(false);

    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        if (!_dataDict.TryGetValue(key, out var _)) return Task.FromResult(false);
        _dataDict.TryRemove(key, out var _);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Delete entities.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        var predicate = filter.Compile();
        var deleteIds = _dataDict.Where(keyValue => predicate(keyValue.Value))
            .Select(it => it.Key)
            .ToList();
        if (deleteIds.Count == 0) return Task.FromResult(false);
        deleteIds.ForEach(key => _dataDict.TryRemove(key, out var _));
        return Task.FromResult(true);
    }

    /// <summary>
    /// Insert entities in bulk operation.
    /// </summary>
    /// <param name="entities">Entities to insert bulk</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task InsertBulkAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (false == (entities?.Any() ?? false)) return;
        await Task.WhenAll(entities.Select(it => UpsertAsync(it.Id, it, cancellationToken)));
    }

    #endregion
}

/// <summary>
/// In-memory implementation of <see cref="IRepository{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <remarks>
/// Initialize an instance of <see cref="InMemoryRepository{TEntity}"/>.
/// </remarks>
/// <param name="mappingStrategy">The mapping strategy</param>
/// <param name="idField">The id field selector</param>
public class InMemoryRepository<TEntity>(IMappingStrategy mappingStrategy, Expression<Func<TEntity, string>> idField)
    : InMemoryRepository<TEntity, string>(mappingStrategy, idField), IInMemoryRepository<TEntity>
    where TEntity : class, IDbModel<string>;