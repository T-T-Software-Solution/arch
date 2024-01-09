using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Infra.Data.Redis.Models;

/// <summary>
/// Resumable Redis setup.
/// </summary>
public sealed record RedisSetup
{
    #region Properties

    internal IServiceCollection ServiceCollection { get; init; }
    internal RedisConnectionStoreBuilder ConnectionStoreBuilder { get; init; }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize a new instance of <see cref="RedisSetup"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="connectionStoreBuilder">The connection store builder</param>
    /// <exception cref="ArgumentNullException">All parameters are required</exception>
    public RedisSetup(IServiceCollection serviceCollection, RedisConnectionStoreBuilder connectionStoreBuilder)
    {
        ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        ConnectionStoreBuilder = connectionStoreBuilder ?? throw new ArgumentNullException(nameof(connectionStoreBuilder));
    }

    #endregion
}