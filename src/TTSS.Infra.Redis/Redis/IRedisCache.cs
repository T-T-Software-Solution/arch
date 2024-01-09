using StackExchange.Redis;
using System.Numerics;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Contract for Redis cache.
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRedisCache<TEntity> : IDisposable
    where TEntity : RedisCacheBase<TEntity>
{
    /// <summary>
    /// Primitive cache object.
    /// </summary>
    TEntity Primitive { get; }

    /// <summary>
    /// Get value from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    Task<RedisValue> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set value to cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task SetAsync(string key, RedisValue value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get multiple-values from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="valueCount">Number of values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    Task<RedisValue[]> GetAsync(string key, int valueCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set multiple-values to cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="values">The values</param>
    /// <returns>Acknowledgement</returns>
    Task SetAsync(string key, CancellationToken cancellationToken = default, params RedisValue[] values);

    /// <summary>
    /// Delete value from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete multiple-values from cache.
    /// </summary>
    /// <param name="keys">The keys</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task DeleteAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increment value in cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="incValue">Increment value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The incremented value</returns>
    Task<long> IncrementAsync(string key, long incValue = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set value to cache.
    /// </summary>
    /// <typeparam name="TContent">The content type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Acknowledgement</returns>
    Task SetAsync<TContent>(string key, TContent value, CancellationToken cancellationToken = default) where TContent : class, new();

    /// <summary>
    /// Get value from cache.
    /// </summary>
    /// <typeparam name="TContent">The content type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    Task<TContent?> GetAsync<TContent>(string key, CancellationToken cancellationToken = default) where TContent : class;

    /// <summary>
    /// Get text from cache.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    Task<string?> GetTextAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get number from cache.
    /// </summary>
    /// <typeparam name="TNumber">Number data type</typeparam>
    /// <param name="key">The key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result</returns>
    Task<TNumber?> GetNumberAsync<TNumber>(string key, CancellationToken cancellationToken = default) where TNumber : INumber<TNumber>;
}