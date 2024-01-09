using System.Collections.Concurrent;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Store for registered Redis connections.
/// </summary>
public class RedisConnectionStore
{
    #region Fields

    private ConcurrentDictionary<string, RedisMultiplexer> _databases = null!;
    private readonly Dictionary<Type, RedisConnection> _connections = new();

    #endregion

    #region Methods

    internal void Add(RedisConnection connection)
        => _connections.Add(connection.CollectionType, connection);

    internal (RedisMultiplexer Multiplexer, RedisCacheBehavior Behavior) GetConfigurations(Type collectionType)
    {
        if (!_connections.TryGetValue(collectionType, out var connection))
            throw new ArgumentOutOfRangeException($"Cache collection '{collectionType}' not found.");

        RedisMultiplexer? client = default;
        if (!_databases?.TryGetValue(connection.ConnectionString, out client) ?? false)
            throw new ArgumentOutOfRangeException($"Redis type '{connection.CollectionType}' not found.");

        return (client!, connection.Behavior!);
    }

    internal RedisConnectionStore Build()
    {
        _databases ??= new ConcurrentDictionary<string, RedisMultiplexer>();
        _connections
            .Select(it => it.Value)
            .Distinct()
            .ToList()
            .ForEach(it => _databases.TryAdd(it.ConnectionString, new(it)));
        return this;
    }

    #endregion
}