using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Redis connection store builder.
/// </summary>
public class RedisConnectionStoreBuilder
{
    #region Fields

    private string _currentConnectionString = null!;
    private RedisConnectionStore _connectionStore = new();

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisConnectionStoreBuilder"/> class.
    /// </summary>
    public RedisConnectionStoreBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisConnectionStoreBuilder"/> class.
    /// </summary>
    /// <param name="connectionString">The Redis connection string</param>
    public RedisConnectionStoreBuilder(string connectionString)
        => SetupDatabase(connectionString);

    #endregion

    #region Methods

    /// <summary>
    /// Setup Redis repository.
    /// </summary>
    /// <param name="connectionString">The Redis connection string</param>
    /// <returns>The <see cref="RedisConnectionStoreBuilder"/> instance</returns>
    /// <exception cref="ArgumentException">The connectionString is required</exception>
    public RedisConnectionStoreBuilder SetupDatabase(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException(nameof(connectionString));
        _currentConnectionString = connectionString;
        return this;
    }

    /// <summary>
    /// Add Redis model for dependency injection.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="behavior">Entity behavior</param>
    /// <returns>The <see cref="RedisConnectionStoreBuilder"/> instance</returns>
    public RedisConnectionStoreBuilder RegisterCollection<TEntity>(RedisCacheBehavior? behavior = default)
        => RegisterCollection(typeof(TEntity), behavior);

    /// <summary>
    /// Add Redis model for dependency injection.
    /// </summary>
    /// <param name="type">The Entity data type</param>
    /// <param name="behavior">Entity behavior</param>
    /// <returns>The <see cref="RedisConnectionStoreBuilder"/> instance</returns>
    /// <exception cref="InvalidOperationException">The SetupConnection() method must be called before setting the collection</exception>
    public RedisConnectionStoreBuilder RegisterCollection(Type type, RedisCacheBehavior? behavior = default)
    {
        if (string.IsNullOrWhiteSpace(_currentConnectionString))
            throw new InvalidOperationException("SetupConnection() must be called before setting the collection.");

        var connection = new RedisConnection(type, _currentConnectionString, behavior);
        _connectionStore.Add(connection);
        return this;
    }

    /// <summary>
    /// Complete Redis setup.
    /// </summary>
    /// <returns>The <see cref="RedisConnectionStore"/> instance</returns>
    public RedisConnectionStore Build()
    {
        var store = _connectionStore.Build();
        _connectionStore = new();
        return store;
    }

    #endregion
}