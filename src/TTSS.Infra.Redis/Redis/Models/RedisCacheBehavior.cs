namespace TTSS.Infra.Data.Redis.Models;

/// <summary>
/// Redis cache behavior.
/// </summary>
public sealed record RedisCacheBehavior
{
    #region Fields

    /// <summary>
    /// Default cache behavior.
    /// </summary>
    public static readonly RedisCacheBehavior Default = new();

    #endregion

    #region Properties

    /// <summary>
    /// Expiry time.
    /// </summary>
    public TimeSpan? Expiry { get; }

    /// <summary>
    /// Prefix for cache key.
    /// </summary>
    public string? KeyPrefix { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initialize a new instance of <see cref="RedisCacheBehavior"/>.
    /// </summary>
    private RedisCacheBehavior()
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="RedisCacheBehavior"/>.
    /// </summary>
    /// <param name="keyPrefix">The key prefix</param>
    /// <exception cref="ArgumentNullException">The KeyPrefix is required</exception>
    public RedisCacheBehavior(string keyPrefix)
        => KeyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));

    /// <summary>
    /// Initialize a new instance of <see cref="RedisCacheBehavior"/>.
    /// </summary>
    /// <param name="expiry">The expiry time</param>
    public RedisCacheBehavior(TimeSpan? expiry)
        => Expiry = expiry;

    /// <summary>
    /// Initialize a new instance of <see cref="RedisCacheBehavior"/>.
    /// </summary>
    /// <param name="keyPrefix">The key prefix</param>
    /// <param name="expiry">The expiry time</param>
    /// <exception cref="ArgumentNullException">The KeyPrefix is required</exception>
    public RedisCacheBehavior(string keyPrefix, TimeSpan? expiry)
    {
        Expiry = expiry;
        KeyPrefix = keyPrefix ?? throw new ArgumentNullException(nameof(keyPrefix));
    }

    #endregion
}