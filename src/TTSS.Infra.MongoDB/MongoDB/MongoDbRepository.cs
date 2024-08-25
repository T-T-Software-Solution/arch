using MongoDB.Driver;
using System.Linq.Expressions;
using TTSS.Core.Data;
using TTSS.Core.Services;

namespace TTSS.Infra.Data.MongoDB;

/// <summary>
/// MongoDb implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class MongoDbRepository<TEntity, TKey> : IMongoDbRepository<TEntity, TKey>
    where TEntity : class, IDbModel<TKey>
    where TKey : notnull
{
    #region Properties

    /// <summary>
    /// Batch size for bulk insert.
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Bypass document validation.
    /// </summary>
    public bool BypassDocumentValidation { get; set; } = true;

    /// <summary>
    /// Mapping strategy.
    /// </summary>
    protected IMappingStrategy MappingStrategy { get; }

    /// <summary>
    /// Id field selector.
    /// </summary>
    protected Expression<Func<TEntity, TKey>> IdField { get; }

    /// <summary>
    /// Working with collection name.
    /// </summary>
    protected internal string CollectionName { get; }

    /// <summary>
    /// MongoDb collection.
    /// </summary>
    protected internal IMongoCollection<TEntity> Collection { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="MongoDbRepository{TEntity, TKey}"/>.
    /// </summary>
    /// <param name="connectionStore">The MongoDB connection store</param>
    /// <param name="mappingStrategy">The mapping strategy</param>
    /// <param name="idField">The id field selector</param>
    /// <exception cref="ArgumentOutOfRangeException">The MongoDbConnectionStore's collection is required</exception>
    /// <exception cref="ArgumentNullException">The idField is required</exception>
    public MongoDbRepository(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy, Expression<Func<TEntity, TKey>>? idField = default)
    {
        MappingStrategy = mappingStrategy;
        IdField = idField ??= it => it.Id;
        var (collectionName, collection) = connectionStore.GetCollection<TEntity>();
        Collection = collection ?? throw new ArgumentOutOfRangeException(nameof(connectionStore), $"The {nameof(collection)} must not be null.");
        CollectionName = collectionName ?? throw new ArgumentOutOfRangeException(nameof(connectionStore), $"The {nameof(collectionName)} must not be null.");
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
    {
        var idKey = GetEntityFilter(key);
        return Collection.Find(idKey).FirstOrDefaultAsync(cancellationToken)!;
    }

    /// <summary>
    /// Get all data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(CancellationToken cancellationToken = default)
        => new MongoDbQueryResult<TEntity>(Collection.Find(Builders<TEntity>.Filter.Empty), MappingStrategy, cancellationToken);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => new MongoDbQueryResult<TEntity>(Collection.Find(filter), MappingStrategy, cancellationToken);

    PagingSet<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize)
        => PagingService.GetPaging(this, pageNo, pageSize);

    PagingSet<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize, Expression<Func<TEntity, bool>> filter)
        => PagingService.GetPaging(this, pageNo, pageSize, filter);

    PagingSet<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize, Action<IPagingRepository<TEntity>> decorate)
        => PagingService.GetPaging(this, pageNo, pageSize, decorate: decorate);

    PagingSet<TEntity> IQueryRepository<TEntity, TKey>.GetPaging(int pageNo, int pageSize, Expression<Func<TEntity, bool>> filter, Action<IPagingRepository<TEntity>> decorate)
        => PagingService.GetPaging(this, pageNo, pageSize, filter, decorate);

    /// <summary>
    /// Convert to queryable.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Queryable</returns>
    public IQueryable<TEntity> Query(CancellationToken cancellationToken = default)
        => Collection.AsQueryable();

    /// <summary>
    /// Insert an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => Collection.InsertOneAsync(entity, new InsertOneOptions
        {
            BypassDocumentValidation = BypassDocumentValidation
        }, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpdateAsync(entity.Id, entity, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task<bool> UpdateAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default)
    {
        if (!key?.Equals(entity.Id) ?? false) return false;
        var idKey = GetEntityFilter(key!);
        var result = await Collection.ReplaceOneAsync(idKey, entity, new ReplaceOptions
        {
            IsUpsert = false,
            BypassDocumentValidation = BypassDocumentValidation,
        }, cancellationToken);
        return result.IsAcknowledged && result.MatchedCount > 0;
    }

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> UpsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        => UpsertAsync(entity.Id, entity, cancellationToken);

    /// <summary>
    /// Update an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to insert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task<bool> UpsertAsync(TKey key, TEntity entity, CancellationToken cancellationToken = default)
    {
        var idKey = GetEntityFilter(key);
        var result = await Collection.ReplaceOneAsync(idKey, entity, new ReplaceOptions
        {
            IsUpsert = true,
            BypassDocumentValidation = BypassDocumentValidation,
        }, cancellationToken);
        return result.IsAcknowledged && (result.ModifiedCount > 0 || result.UpsertedId != null);
    }

    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task<bool> DeleteAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var idKey = GetEntityFilter(key);
        var result = await Collection.DeleteOneAsync(idKey, cancellationToken);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    /// <summary>
    /// Delete entities.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task<bool> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        var result = await Collection.DeleteManyAsync<TEntity>(filter, cancellationToken);
        return result.IsAcknowledged && result.DeletedCount > 0;
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
        var batch = entities.Take(BatchSize).ToList();
        entities = entities.Skip(BatchSize).ToList();

        while (batch.Count != 0)
        {
            var startRequestDateTime = DateTime.Now;

            try
            {
                await Collection.InsertManyAsync(batch,
                    new InsertManyOptions { BypassDocumentValidation = BypassDocumentValidation },
                    cancellationToken);
            }
            catch (MongoBulkWriteException<TEntity> ex)
            {
                // in case of request rate limit
                if (ex.WriteErrors.Count > 0 && ex.WriteErrors.Any(it => it.Code == 16500))
                {
                    batch = batch.Skip((int)ex.Result.InsertedCount).ToList();
                    var now = DateTime.Now;
                    var usedTime = now - startRequestDateTime;
                    var mustWait = Math.Min(9, 1009 - usedTime.Milliseconds);
                    var backOff = Convert.ToInt32(now.Ticks % 30);
                    await Task.Delay(mustWait + backOff, cancellationToken);
                    continue;
                }
                else
                {
                    throw;
                }
            }
            batch = entities.Take(BatchSize).ToList();
            entities = entities.Skip(BatchSize).ToList();
        }
    }

    /// <summary>
    /// Get entity filter by key.
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>Filter definition</returns>
    protected virtual FilterDefinition<TEntity> GetEntityFilter(TKey key)
        => Builders<TEntity>.Filter.Eq(IdField, key);

    #endregion
}

/// <summary>
/// MongoDb implementation of <see cref="IRepository{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <param name="mappingStrategy">The mapping strategy</param>
/// <remarks>
/// Initialize an instance of <see cref="MongoDbRepository{T}"/>.
/// </remarks>
/// <param name="connectionStore">The connection store</param>
public class MongoDbRepository<TEntity>(MongoDbConnectionStore connectionStore, IMappingStrategy mappingStrategy) : MongoDbRepository<TEntity, string>(connectionStore, mappingStrategy, it => it.Id),
    IMongoDbRepository<TEntity>
    where TEntity : class, IDbModel<string>;