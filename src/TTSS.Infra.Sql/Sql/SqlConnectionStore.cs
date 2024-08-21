using Microsoft.EntityFrameworkCore;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Store for registerd SQL connections.
/// </summary>
public sealed class SqlConnectionStore
{
    #region Fields

    private SqlInterceptorBuilder? _builder;
    private readonly Dictionary<string, SqlConnection> _connections = [];

    #endregion

    #region Methods

    internal void Add(SqlConnection connection)
        => _connections.Add(connection.TypeName, connection);

    internal void SetInterceptors(SqlInterceptorBuilder builder)
        => _builder = builder ?? throw new ArgumentNullException(nameof(builder));

    internal (DbSet<TEntity>? collection, DbContext? dbContext) GetCollection<TEntity>(SqlDbContextFactory dbContextFactory)
        where TEntity : class
    {
        var typeName = typeof(TEntity).Name;
        if (!_connections.TryGetValue(typeName, out var connection))
            throw new ArgumentOutOfRangeException($"Collection '{typeName}' not found.");

        var dbContext = dbContextFactory.GetDbContext(connection.DbContextDataType);
        var interceptors = (_builder is null) ? [] : dbContextFactory.GetInterceptors(this, _builder).ToList();
        if (interceptors.Any() && dbContext is DbContextBase contextBase)
        {
            contextBase.SetInterceptors(interceptors);
        }

        var collection = dbContext.Set<TEntity>();
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();
        return (collection, dbContext);
    }

    #endregion
}