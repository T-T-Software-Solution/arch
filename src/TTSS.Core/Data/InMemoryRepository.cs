using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace TTSS.Core.Data;

/// <summary>
/// In-memory implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class InMemoryRepository<TEntity, TKey> : IInMemoryRepository<TEntity, TKey>
    where TEntity : IDbModel<TKey>
    where TKey : notnull
{
    #region Fields

    private readonly ConcurrentDictionary<TKey, TEntity> _dataDict;

    #endregion

    #region Properties

    private Func<TEntity, TKey> GetKey { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="InMemoryRepository{TEntity, TKey}"/>.
    /// </summary>
    /// <param name="idField">The id field selector</param>
    /// <exception cref="ArgumentNullException">The id field selector is required</exception>
    public InMemoryRepository(Expression<Func<TEntity, TKey>> idField)
    {
        if (idField is null) throw new ArgumentNullException(nameof(idField));
        _dataDict = new();
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
        => new InMemoryQueryResult<TEntity>(_dataDict.Values);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => new InMemoryQueryResult<TEntity>(_dataDict.Values.Where(filter.Compile()));

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
        if (!deleteIds.Any()) return Task.FromResult(false);
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
public class InMemoryRepository<TEntity> : InMemoryRepository<TEntity, string>,
    IInMemoryRepository<TEntity>
    where TEntity : IDbModel<string>
{
    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="InMemoryRepository{TEntity}"/>.
    /// </summary>
    /// <param name="idField">The id field selector</param>
    public InMemoryRepository(Expression<Func<TEntity, string>> idField) : base(idField)
    {
    }

    #endregion
}