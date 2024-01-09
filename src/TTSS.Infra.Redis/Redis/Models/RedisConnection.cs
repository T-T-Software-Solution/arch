namespace TTSS.Infra.Data.Redis.Models;

/// <summary>
/// Redis connection information.
/// </summary>
public sealed record RedisConnection
{
    #region Properties

    /// <summary>
    /// Collection type.
    /// </summary>
    public Type CollectionType { get; init; }

    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString { get; init; }

    /// <summary>
    /// Store behavior.
    /// </summary>
    public RedisCacheBehavior? Behavior { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisConnection"/> class.
    /// </summary>
    /// <param name="collectionType">Type of the collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="behavior">Repository behavior</param>
    public RedisConnection(Type collectionType, string connectionString, RedisCacheBehavior? behavior)
    {
        CollectionType = collectionType;
        ConnectionString = connectionString;
        Behavior = behavior;
    }

    #endregion
}