using Microsoft.EntityFrameworkCore;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// SQL connection store builder.
/// </summary>
public class SqlConnectionStoreBuilder
{
    #region Fields

    private Type _dbContextDataType = null!;
    private SqlConnectionStore _connectionStore = new();

    #endregion

    #region Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionStoreBuilder"/> class.
    /// </summary>
    /// <typeparam name="TDbContext">Type of DbContext</typeparam>
    /// <returns>The <see cref="SqlConnectionStoreBuilder"/> instance</returns>
    public SqlConnectionStoreBuilder SetupDatabase<TDbContext>() where TDbContext : DbContext
        => SetupDatabase(typeof(TDbContext));

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlConnectionStoreBuilder"/> class.
    /// </summary>
    /// <param name="type">Type of DbContext</param>
    /// <returns>The <see cref="SqlConnectionStoreBuilder"/> instance</returns>
    /// <exception cref="ArgumentOutOfRangeException">The data type must be a subclass of DbContext</exception>
    public SqlConnectionStoreBuilder SetupDatabase(Type type)
    {
        if (!type.IsSubclassOf(typeof(DbContext))) throw new ArgumentOutOfRangeException($"{nameof(type)} must be a subclass of DbContext.");
        _dbContextDataType = type;
        return this;
    }

    /// <summary>
    /// Register a collection.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <returns>The <see cref="SqlConnectionStoreBuilder"/> instance</returns>
    public SqlConnectionStoreBuilder RegisterCollection<TEntity>() where TEntity : IDbModel
        => RegisterCollection(typeof(TEntity));

    /// <summary>
    /// Register a collection.
    /// </summary>
    /// <param name="entityType">Entity type</param>
    /// <returns>The <see cref="SqlConnectionStoreBuilder"/> instance</returns>
    /// <exception cref="ArgumentOutOfRangeException">The collection type must be a subclass of SqlModelBase</exception>
    public SqlConnectionStoreBuilder RegisterCollection(Type? entityType)
    {
        if (false == (entityType?.IsAssignableTo(typeof(IDbModel)) ?? false))
            throw new ArgumentOutOfRangeException($"{nameof(entityType)} must implement IDbModel");

        var connection = new SqlConnection(entityType, _dbContextDataType);
        _connectionStore.Add(connection);
        return this;
    }

    /// <summary>
    /// Build the connection store.
    /// </summary>
    /// <returns>The <see cref="SqlConnectionStore"/> instance</returns>
    public SqlConnectionStore Build()
    {
        var store = _connectionStore;
        _connectionStore = new();
        return store;
    }

    #endregion
}