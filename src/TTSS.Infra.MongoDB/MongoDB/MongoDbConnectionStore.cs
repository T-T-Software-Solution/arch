using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Concurrent;
using TTSS.Infra.Data.MongoDB.Models;

namespace TTSS.Infra.Data.MongoDB;

/// <summary>
/// Store for registered MongoDB connections.
/// </summary>
public sealed class MongoDbConnectionStore
{
    #region Fields

    private ConcurrentDictionary<string, MongoClient> _clients = null!;
    private readonly Dictionary<string, MongoDbConnection> _connections = new();

    #endregion

    #region Methods

    internal void Add(MongoDbConnection connection)
        => _connections.TryAdd(connection.TypeName, connection);

    internal (string? collectionName, IMongoCollection<T>? collection) GetCollection<T>()
    {
        var typeName = typeof(T).Name;
        if (!_connections.TryGetValue(typeName, out var connection))
            throw new ArgumentOutOfRangeException($"Collection '{typeName}' not found.");

        MongoClient? client = default;
        if (!_clients?.TryGetValue(connection.ConnectionString, out client) ?? false)
            throw new ArgumentOutOfRangeException($"Database '{connection.DatabaseName}' not found.");

        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
        {
            BsonClassMap.TryRegisterClassMap<T>(it =>
            {
                it.AutoMap();
                it.SetIsRootClass(!connection.IsChild);
            });
        }

        var database = client?.GetDatabase(connection.DatabaseName);
        var collection = database?.GetCollection<T>(connection.CollectionName);
        return (connection.CollectionName, connection.NoDiscriminator ? collection : collection?.OfType<T>());
    }

    internal MongoDbConnectionStore Build()
    {
        _clients ??= new ConcurrentDictionary<string, MongoClient>();
        _connections
            .Select(it => it.Value.ConnectionString)
            .Distinct()
            .ToList()
            .ForEach(connString =>
            {
                var client = new MongoClient(connString);
                _clients.TryAdd(connString, client);
            });
        return this;
    }

    #endregion
}