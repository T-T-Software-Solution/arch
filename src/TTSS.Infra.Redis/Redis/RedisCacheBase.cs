using StackExchange.Redis;
using System.Numerics;
using System.Text;
using System.Text.Json;
using TTSS.Core;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Base class for Redis cache.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class RedisCacheBase<TEntity> : IRedisCache<TEntity>
    where TEntity : RedisCacheBase<TEntity>
{
    #region Fields

    private readonly string _keyPrefix;
    private readonly RedisMultiplexer _multiplexer;

    #endregion

    #region Properties

    /// <summary>
    /// Primitive cache object.
    /// </summary>
    public TEntity Primitive => (TEntity)this;

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
    /// Initialize a new instance of <see cref="RedisCacheBase{TEntity}"/>.
    /// </summary>
    /// <param name="connectionStore">The Redis connection store</param>
    public RedisCacheBase(RedisConnectionStore connectionStore)
    {
        var typeName = GetType().Name;
        var config = connectionStore.GetConfigurations(GetType());
        Behavior = GetCacheBehavior(config.Behavior ?? RedisCacheBehavior.Default) ?? RedisCacheBehavior.Default;

        var builder = new StringBuilder();
        if (false == string.IsNullOrWhiteSpace(Behavior.KeyPrefix))
        {
            builder.Append($"{Behavior.KeyPrefix}:");
        }
        builder.Append($"{typeName}");
        _keyPrefix = builder.ToString();
        _multiplexer = config.Multiplexer;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get value from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    public async Task<RedisValue> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        key = GetKey(key);

        return await redis.StringGetAsync(key);
    }

    /// <summary>
    /// Set value to cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task SetAsync(string key, RedisValue value, CancellationToken cancellationToken = default)
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        key = GetKey(key);

        await redis.StringSetAsync(key, value, Behavior.Expiry);
    }

    /// <summary>
    /// Get multiple-values from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="valueCount">Number of values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    public async Task<RedisValue[]> GetAsync(string key, int valueCount, CancellationToken cancellationToken = default)
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        key = GetKey(key);

        var keys = Enumerable.Range(1, valueCount).Select(i => (RedisValue)$"sk{i}").ToArray();
        return await redis.HashGetAsync(key, keys);
    }

    /// <summary>
    /// Set multiple-values to cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="values">The values</param>
    /// <returns>Acknowledgement</returns>
    public async Task SetAsync(string key, CancellationToken cancellationToken = default, params RedisValue[] values)
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        key = GetKey(key);

        var indexes = Enumerable.Range(1, values.Length);
        var entries = indexes.Zip(values, (i, v) => new HashEntry($"sk{i}", v)).ToArray();

        await redis.HashSetAsync(key, entries);
        if (Behavior.Expiry.HasValue) await redis.KeyExpireAsync(key, Behavior.Expiry);
    }

    /// <summary>
    /// Delete value from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        => DeleteAsync([key], cancellationToken);

    /// <summary>
    /// Delete multiple-values from cache.
    /// </summary>
    /// <param name="keys">The keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        var qry = keys
            .Select(GetKey)
            .Where(it => it is not null)
            .Distinct()
            .Select(it => redis.KeyDeleteAsync(it));
        await Task.WhenAll(qry);
    }

    /// <summary>
    /// Increment value in cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="incValue">Increment value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The incremented value</returns>
    public async Task<long> IncrementAsync(string key, long incValue = 1, CancellationToken cancellationToken = default)
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        key = GetKey(key);

        var inc = await redis.StringIncrementAsync(key, incValue);
        if (Behavior.Expiry.HasValue) await redis.KeyExpireAsync(key, Behavior.Expiry);

        return inc;
    }

    /// <summary>
    /// Set value to cache.
    /// </summary>
    /// <typeparam name="TContent">The content type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    public async Task SetAsync<TContent>(string key, TContent value, CancellationToken cancellationToken = default)
        where TContent : class, new()
    {
        var redis = await _multiplexer.GetDatabaseAsync(cancellationToken);
        key = GetKey(key);

        var data = JsonSerializer.Serialize(value, Standard.Json.DefaultSerializerOptions);
        await redis.StringSetAsync(key, data, Behavior.Expiry);
    }

    /// <summary>
    /// Get value from cache.
    /// </summary>
    /// <typeparam name="TContent">The content type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    public async Task<TContent?> GetAsync<TContent>(string key, CancellationToken cancellationToken = default)
        where TContent : class
    {
        var value = await GetAsync(key, cancellationToken);
        if (value.IsNull) return default;
        if (string.IsNullOrWhiteSpace(value)) return value.ToString() as TContent;

        var isJsonFormat = value.StartsWith("{") && value.ToString().EndsWith("}");
        if (false == isJsonFormat) return value.ToString() as TContent;

        return JsonSerializer.Deserialize<TContent>(value.ToString(), Standard.Json.DefaultSerializerOptions);
    }

    /// <summary>
    /// Get text from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    public async Task<string?> GetTextAsync(string key, CancellationToken cancellationToken = default)
        => await GetAsync(key, cancellationToken);

    /// <summary>
    /// Get number from cache.
    /// </summary>
    /// <typeparam name="TNumber">Number data type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    public async Task<TNumber?> GetNumberAsync<TNumber>(string key, CancellationToken cancellationToken = default)
        where TNumber : INumber<TNumber>
    {
        var text = await GetTextAsync(key, cancellationToken);
        return TNumber.TryParse(text, null, out var value) ? value : default;
    }

    /// <summary>
    /// Get the key selector
    /// </summary>
    /// <param name="key">The key</param>
    /// <returns>The key selector</returns>
    protected string GetKey(string key) => $"{_keyPrefix}:{key}";

    /// <summary>
    /// Decorate cache behavior.
    /// </summary>
    /// <param name="currentBehavior">Cache behavior</param>
    /// <returns>The decorated cache behavior</returns>
    protected virtual RedisCacheBehavior GetCacheBehavior(RedisCacheBehavior currentBehavior)
        => currentBehavior;

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