using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Store for registerd SQL connections.
/// </summary>
public sealed class SqlConnectionStore
{
    #region Fields

    private IEnumerable<IDbInterceptor>? _interceptors;
    private readonly Dictionary<string, SqlConnection> _connections = [];

    #endregion

    #region Methods

    internal void Add(SqlConnection connection)
        => _connections.Add(connection.TypeName, connection);

    internal void SetInterceptors(IEnumerable<IDbInterceptor> interceptors)
        => _interceptors = interceptors?.Where(it => it is not null) ?? throw new ArgumentNullException(nameof(interceptors));

    internal (DbSet<TEntity>? collection, DbContext? dbContext) GetCollection<TEntity>(SqlDbContextFactory dbContextFactory)
        where TEntity : class
    {
        var typeName = typeof(TEntity).Name;
        if (!_connections.TryGetValue(typeName, out var connection))
            throw new ArgumentOutOfRangeException($"Collection '{typeName}' not found.");

        var dbContext = dbContextFactory.GetDbContext(connection.DbContextDataType);
        if (_interceptors is not null && dbContext is DbContextBase contextBase)
        {
            var interceptors = _interceptors
                .Where(it => it is not null && it is IInterceptor)
                .Cast<IInterceptor>();
            contextBase.SetInterceptors(interceptors);
        }
        var collection = dbContext.Set<TEntity>();
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();
        return (collection, dbContext);
    }

    #endregion
}