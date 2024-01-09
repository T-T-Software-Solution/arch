using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TTSS.Infra.Data.Sql.Models;

/// <summary>
/// Resumable SQL setup.
/// </summary>
public sealed record SqlSetup
{
    #region Fields

    internal IServiceCollection ServiceCollection { get; init; }
    internal SqlConnectionStoreBuilder ConnectionStoreBuilder { get; init; }
    internal Action<DbContextOptionsBuilder> DatabaseContextConfig { get; init; }

    #endregion

    #region Constructors

    internal SqlSetup(IServiceCollection serviceCollection, SqlConnectionStoreBuilder connectionStoreBuilder, Action<DbContextOptionsBuilder> databaseContextConfig)
    {
        ServiceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        ConnectionStoreBuilder = connectionStoreBuilder ?? throw new ArgumentNullException(nameof(connectionStoreBuilder));
        DatabaseContextConfig = databaseContextConfig ?? throw new ArgumentNullException(nameof(databaseContextConfig));
    }

    #endregion
}