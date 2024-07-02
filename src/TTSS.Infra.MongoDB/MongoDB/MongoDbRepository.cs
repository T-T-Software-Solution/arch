using MongoDB.Driver;
using System.Linq.Expressions;
using TTSS.Core.Data;

namespace TTSS.Infra.Data.MongoDB;

/// <summary>
/// MongoDb implementation of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <typeparam name="TKey">Primary key type</typeparam>
public class MongoDbRepository<TEntity, TKey> : IMongoDbRepository<TEntity, TKey>
    where TEntity : IDbModel<TKey>
    where TKey : notnull
{
    #region Fields

    /// <summary>
    /// Batch size for bulk insert.
    /// </summary>
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Bypass document validation.
    /// </summary>
    public bool BypassDocumentValidation { get; set; } = true;

    /// <summary>
    /// Working with collection name.
    /// </summary>
    protected internal readonly string CollectionName;

    /// <summary>
    /// MongoDb collection.
    /// </summary>
    protected internal readonly IMongoCollection<TEntity> Collection;

    private readonly Expression<Func<TEntity, TKey>> _idField;

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize an instance of <see cref="MongoDbRepository{TEntity, TKey}"/>.
    /// </summary>
    /// <param name="connectionStore">The MongoDB connection store</param>
    /// <param name="idField">The id field selector</param>
    /// <exception cref="ArgumentOutOfRangeException">The MongoDbConnectionStore's collection is required</exception>
    /// <exception cref="ArgumentNullException">The idField is required</exception>
    public MongoDbRepository(MongoDbConnectionStore connectionStore, Expression<Func<TEntity, TKey>>? idField = default)
    {
        idField ??= it => it.Id;
        var (collectionName, collection) = connectionStore.GetCollection<TEntity>();
        Collection = collection ?? throw new ArgumentOutOfRangeException(nameof(connectionStore), $"The {nameof(collection)} must not be null.");
        CollectionName = collectionName ?? throw new ArgumentOutOfRangeException(nameof(connectionStore), $"The {nameof(collectionName)} must not be null.");
        _idField = idField ?? throw new ArgumentNullException(nameof(idField));
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
        => new MongoDbQueryResult<TEntity>(Collection.Find(Builders<TEntity>.Filter.Empty), cancellationToken);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filter">Entity filter</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        => new MongoDbQueryResult<TEntity>(Collection.Find(filter), cancellationToken);

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
        => Builders<TEntity>.Filter.Eq(_idField, key);

    #endregion
}

/// <summary>
/// MongoDb implementation of <see cref="IRepository{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
/// <remarks>
/// Initialize an instance of <see cref="MongoDbRepository{T}"/>.
/// </remarks>
/// <param name="connectionStore">The connection store</param>
public class MongoDbRepository<TEntity>(MongoDbConnectionStore connectionStore) : MongoDbRepository<TEntity, string>(connectionStore, it => it.Id),
    IMongoDbRepository<TEntity>
    where TEntity : IDbModel<string>;