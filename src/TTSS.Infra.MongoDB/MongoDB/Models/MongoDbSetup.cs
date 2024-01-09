using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Infra.Data.MongoDB.Models;

/// <summary>
/// Resumable MongoDB setup.
/// </summary>
public sealed class MongoDbSetup
{
    #region Fields

    internal IServiceCollection ServiceCollection { get; init; }
    internal MongoDbConnectionStoreBuilder ConnectionStoreBuilder { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbSetup"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="connectionStoreBuilder">The connection store builder</param>
    /// <exception cref="ArgumentNullException">All parameters are required</exception>
    internal MongoDbSetup(IServiceCollection serviceCollection, MongoDbConnectionStoreBuilder connectionStoreBuilder)
    {
        ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        ConnectionStoreBuilder = connectionStoreBuilder ?? throw new ArgumentNullException(nameof(connectionStoreBuilder));
    }

    #endregion
}