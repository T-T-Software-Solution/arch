using StackExchange.Redis;
using System.Text;
using TTSS.Core;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Base class for Redis repository cache.
/// </summary>
public abstract class RedisRepositoryCache;

/// <summary>
/// Redis repository cache.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public class RedisRepositoryCache<TEntity> : RedisRepositoryCache,
    IRedisRepositoryCache<TEntity>
    where TEntity : class
{
    #region Fields

    private readonly string _keyPrefix;
    private readonly RedisMultiplexer _multiplexer;
    private const string CounterId = nameof(CounterId);

    #endregion

    #region Properties

    /// <summary>
    /// Cache behavior.
    /// </summary>
    protected RedisCacheBehavior Behavior { get; set; }

    /// <summary>
    /// Redis database for test purpose.
    /// </summary>
    internal ValueTask<IDatabase> Redis => _multiplexer.GetDatabaseAsync();

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize a new instance of <see cref="RedisRepositoryCache{TEntity}"/>.
    /// </summary>
    /// <param name="connectionStore">The Redis connection store</param>
    public RedisRepositoryCache(RedisConnectionStore connectionStore)
    {
        var type = GetType();
        var config = connectionStore.GetConfigurations(type);
        Behavior = GetCacheBehavior(config.Behavior ?? RedisCacheBehavior.Default) ?? RedisCacheBehavior.Default;

        var builder = new StringBuilder();
        if (false == string.IsNullOrWhiteSpace(Behavior.KeyPrefix))
        {
            builder.Append($"{Behavior.KeyPrefix}:");
        }
        const char Separater = '_';
        builder
            .Append(type.Name[..^2])
            .Append(Separater)
            .Append(string.Join(Separater, type.GetGenericArguments().Select(it => it.Name)));
        _keyPrefix = builder.ToString();
        _multiplexer = config.Multiplexer;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get an entity by id.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity</returns>
    public Task<TEntity?> GetByIdAsync(string key, CancellationToken cancellationToken = default)
        => Execute(key, (k, db) => ConvertToEntity(db.StringGetAsync(k)), cancellationToken);

    /// <summary>
    /// Get data by filter.
    /// </summary>
    /// <param name="filterKeys">Filter keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entities</returns>
    public Task<IEnumerable<TEntity>> GetAsync(IEnumerable<string> filterKeys, CancellationToken cancellationToken = default)
        => Execute(filterKeys, (k, db) => ConvertToEntity(db.StringGetAsync(k)), cancellationToken)!;

    /// <summary>
    /// Upsert an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="entity">Entity to upsert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> SetAsync(string key, TEntity entity, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(key)
            ? Task.FromResult(false)
            : Execute(key, (k, db) =>
            {
                var data = Standard.Json.Serialize(entity);
                return db.StringSetAsync(k, data, Behavior.Expiry);
            }, cancellationToken);

    /// <summary>
    /// Delete an entity.
    /// </summary>
    /// <param name="key">Target entity key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default)
        => DeleteManyAsync([key], cancellationToken);

    /// <summary>
    /// Delete entities.
    /// </summary>
    /// <param name="filterKeys">Filter keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> DeleteManyAsync(IEnumerable<string> filterKeys, CancellationToken cancellationToken = default)
        => Execute(filterKeys, async (k, db) => await db.KeyDeleteAsync(k) > 0, cancellationToken);

    /// <summary>
    /// Increase the value of the counter for the repository in the cache.
    /// </summary>
    /// <param name="incValue">Increment value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The incremented value</returns>
    public Task<long> IncrementCounterAsync(long incValue = 1, CancellationToken cancellationToken = default)
        => Execute(CounterId, async (k, db) =>
        {
            var inc = await db.StringIncrementAsync(k, incValue);
            if (Behavior.Expiry.HasValue) await db.KeyExpireAsync(k, Behavior.Expiry);
            return inc;
        }, cancellationToken);

    /// <summary>
    /// Decrease the value of the counter for the repository in the cache.
    /// </summary>
    /// <param name="decValue">Decrement value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The decremented value</returns>
    public Task<long> DecrementCounterAsync(long decValue = 1, CancellationToken cancellationToken = default)
        => Execute(CounterId, async (k, db) =>
        {
            var dec = await db.StringDecrementAsync(k, decValue);
            if (Behavior.Expiry.HasValue) await db.KeyExpireAsync(k, Behavior.Expiry);
            return dec;
        }, cancellationToken);

    /// <summary>
    /// Reset the value of the counter for the repository in the cache.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task<bool> ResetCounter(CancellationToken cancellationToken = default)
         => Execute(CounterId, async (k, db) =>
         {
             var ack = await db.StringSetAsync(k, 0);
             if (Behavior.Expiry.HasValue) await db.KeyExpireAsync(k, Behavior.Expiry);
             return ack;
         }, cancellationToken);

    /// <summary>
    /// Get the key selector
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>The key selector</returns>
    protected RedisKey GetKey(string key) => $"{_keyPrefix}:{key}";

    /// <summary>
    /// Get the key selectors
    /// </summary>
    /// <param name="keys">The keys</param>
    /// <returns>The key selectors</returns>
    protected RedisKey[] GetKey(IEnumerable<string> keys)
        => keys.Where(it => !string.IsNullOrWhiteSpace(it)).Select(GetKey).Distinct().ToArray();

    /// <summary>
    /// Execute a Redis command.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="callback">Actual execution logic</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result</returns>
    protected async Task<TResponse> Execute<TResponse>(string key, Func<RedisKey, IDatabase, Task<TResponse>> callback, CancellationToken cancellationToken)
    {
        var db = await _multiplexer.GetDatabaseAsync(cancellationToken);
        return await callback(GetKey(key), db);
    }

    /// <summary>
    /// Execute a Redis command.
    /// </summary>
    /// <typeparam name="TResponse">Response type</typeparam>
    /// <param name="filterKeys">The filter keys</param>
    /// <param name="callback">Actual execution logic</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result</returns>
    protected async Task<TResponse> Execute<TResponse>(IEnumerable<string> filterKeys, Func<RedisKey[], IDatabase, Task<TResponse>> callback, CancellationToken cancellationToken)
    {
        var db = await _multiplexer.GetDatabaseAsync(cancellationToken);
        var k = GetKey(filterKeys);
        return await callback(k, db);
    }

    /// <summary>
    /// Decorate cache behavior.
    /// </summary>
    /// <param name="currentBehavior">Cache behavior</param>
    /// <returns>The decorated cache behavior</returns>
    protected virtual RedisCacheBehavior GetCacheBehavior(RedisCacheBehavior currentBehavior)
        => currentBehavior;

    private async Task<IEnumerable<TEntity?>> ConvertToEntity(Task<RedisValue[]> tasks)
        => (await tasks).Select(ConvertToEntity);
    private async Task<TEntity?> ConvertToEntity(Task<RedisValue> task)
        => ConvertToEntity(await task);
    private TEntity? ConvertToEntity(RedisValue value)
    {
        if (value.IsNull) return default;
        if (string.IsNullOrWhiteSpace(value)) return value.ToString() as TEntity;

        var isJsonFormat = value.StartsWith("{") && value.ToString().EndsWith('}');
        if (false == isJsonFormat) return value.ToString() as TEntity;
        return Standard.Json.Deserialize<TEntity>(value.ToString());
    }

    /// <summary>
    /// Release resources.
    /// </summary>
    public void Dispose()
    {
        _multiplexer.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}