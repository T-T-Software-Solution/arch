﻿using Microsoft.EntityFrameworkCore;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Store for registerd SQL connections.
/// </summary>
public sealed class SqlConnectionStore
{
    #region Fields

    private readonly Dictionary<string, SqlConnection> _connections = new();

    #endregion

    #region Methods

    internal void Add(SqlConnection connection)
        => _connections.Add(connection.TypeName, connection);

    internal (DbSet<T>? collection, DbContext? dbContext) GetCollection<T>(SqlDbContextFactory dbContextFactory)
        where T : class
    {
        var typeName = typeof(T).Name;
        if (!_connections.TryGetValue(typeName, out var connection))
            throw new ArgumentOutOfRangeException($"Collection '{typeName}' not found.");

        var dbContext = dbContextFactory.GetDbContext(connection.DbContextDataType);
        var collection = dbContext.Set<T>();
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();
        return (collection, dbContext);
    }

    #endregion
}