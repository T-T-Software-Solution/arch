using TTSS.Core.Data;

namespace TTSS.Infra.Data.MongoDB;

/// <summary>
/// MongoDB connection store builder.
/// </summary>
public sealed class MongoDbConnectionStoreBuilder
{
    #region Fields

    private string _currentDatabaseName = null!;
    private string _currentConnectionString = null!;
    private MongoDbConnectionStore _connectionStore = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConnectionStoreBuilder"/> class.
    /// </summary>
    public MongoDbConnectionStoreBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConnectionStoreBuilder"/> class.
    /// </summary>
    /// <param name="databaseName">The database name</param>
    /// <param name="connectionString">The MongoDB connection string</param>
    internal MongoDbConnectionStoreBuilder(string databaseName, string connectionString)
        => SetupDatabase(databaseName, connectionString);

    #endregion

    #region Methods

    /// <summary>
    /// Setup the database name and connection string.
    /// </summary>
    /// <param name="databaseName">The database name</param>
    /// <param name="connectionString">The MongoDB connection string</param>
    /// <returns>The <see cref="MongoDbConnectionStoreBuilder"/> instance</returns>
    /// <exception cref="ArgumentException">All parameters are required</exception>
    public MongoDbConnectionStoreBuilder SetupDatabase(string databaseName, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException(nameof(databaseName));
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(nameof(connectionString));

        _currentDatabaseName = databaseName;
        _currentConnectionString = connectionString;
        return this;
    }

    /// <summary>
    /// Register a collection.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="collectionName">Target collection name</param>
    /// <param name="noDiscriminator">Configures the collection to not use a discriminator</param>
    /// <param name="isChild">Indicates if the collection is a child of another collection</param>
    /// <returns>The <see cref="MongoDbConnectionStoreBuilder"/> instance</returns>
    public MongoDbConnectionStoreBuilder RegisterCollection<TEntity>(string? collectionName = default, bool noDiscriminator = default, bool isChild = default) where TEntity : IDbModel
        => RegisterCollection((string.IsNullOrWhiteSpace(collectionName) ? typeof(TEntity).Name : collectionName), typeof(TEntity).Name, noDiscriminator, isChild);

    /// <summary>
    /// Register a collection.
    /// </summary>
    /// <param name="collectionName">Target collection name</param>
    /// <param name="typeName">Entity type name</param>
    /// <param name="noDiscriminator">Configures the collection to not use a discriminator</param>
    /// <param name="isChild">Indicates if the collection is a child of another collection</param>
    /// <returns>The <see cref="MongoDbConnectionStoreBuilder"/> instance</returns>
    /// <exception cref="InvalidOperationException">Must call SetupDatabase() before setting the collection</exception>
    public MongoDbConnectionStoreBuilder RegisterCollection(string collectionName, string typeName, bool noDiscriminator = default, bool isChild = false)
    {
        if (string.IsNullOrWhiteSpace(_currentDatabaseName) || string.IsNullOrWhiteSpace(_currentConnectionString))
            throw new InvalidOperationException("SetupDatabase() must be called before setting the collection.");

        _connectionStore.Add(new()
        {
            CollectionName = collectionName,
            TypeName = typeName,
            ConnectionString = _currentConnectionString,
            DatabaseName = _currentDatabaseName,
            IsChild = isChild,
            NoDiscriminator = noDiscriminator,
        });
        return this;
    }

    /// <summary>
    /// Build the connection store.
    /// </summary>
    /// <returns>The <see cref="MongoDbConnectionStore"/> instance</returns>
    public MongoDbConnectionStore Build()
    {
        var store = _connectionStore.Build();
        _connectionStore = new();
        return store;
    }

    #endregion
}