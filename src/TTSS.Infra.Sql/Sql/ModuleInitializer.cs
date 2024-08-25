using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Infra.Data.Sql;
using TTSS.Infra.Data.Sql.Models;

namespace TTSS.Infra.Data.Sql;

/// <summary>
/// Helper extension methods for reigster SQL repository.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Setup SQL repository.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="dbContextConfig">The configuration</param>
    /// <returns>Resumable SQL setup</returns>
    public static SqlSetup SetupSqlDatabase(this IServiceCollection target, Action<DbContextOptionsBuilder> dbContextConfig)
    {
        target.AddScoped<SqlDbContextFactory>();
        target.AddScoped<Lazy<IServiceProvider>>(pvd => new Lazy<IServiceProvider>(() => pvd));
        return new(target, new(), dbContextConfig);
    }

    /// <summary>
    /// Setup SQL repository.
    /// </summary>
    /// <param name="target">The SQL setup</param>
    /// <param name="dbContextConfig">The configuration</param>
    /// <returns>Resumable SQL setup</returns>
    public static SqlSetup SetupSqlDatabase(this SqlSetup target, Action<DbContextOptionsBuilder> dbContextConfig)
        => target with { DatabaseContextConfig = dbContextConfig };

    /// <summary>
    /// Add SQL context for dependency injection.
    /// </summary>
    /// <typeparam name="TDbContext">Type of SQL DbContext</typeparam>
    /// <param name="target">The SQL setup</param>
    /// <returns>Resumable SQL setup</returns>
    /// <exception cref="ArgumentNullException">The connection store builder is required</exception>
    /// <exception cref="ArgumentOutOfRangeException">Entity models must implement <see cref="IDbModel"/> or <see cref="IDeletableRepository{TEntity, TKey}"/>.</exception>
    public static SqlSetup AddDbContext<TDbContext>(this SqlSetup target)
        where TDbContext : DbContext, IDbWarmup
    {
        target.ServiceCollection.AddScoped<IDbWarmup, TDbContext>();
        ArgumentNullException.ThrowIfNull(target);
        var builder = target.ConnectionStoreBuilder ?? throw new ArgumentNullException(nameof(target), $"The {nameof(target.ConnectionStoreBuilder)} must not be null.");
        var services = target.ServiceCollection ?? throw new ArgumentNullException(nameof(target), $"The {nameof(target.ServiceCollection)} must not be null.");

        builder.SetupDatabase<TDbContext>();
        services.AddDbContext<TDbContext>(builder => target.DatabaseContextConfig?.Invoke(builder));

        var targetPropertyType = typeof(DbSet<>);
        var targetDbContractType = typeof(IDbModel<>);
        var dbContextType = typeof(TDbContext);
        builder.SetupDatabase(dbContextType);
        foreach (var item in getDbSets(dbContextType)) builder.RegisterCollection(item);
        registerRepository(dbContextType, typeof(IRepository<>), typeof(ISqlRepository<>), typeof(SqlRepository<>));
        registerRepository(dbContextType, typeof(IRepository<,>), typeof(ISqlRepository<,>), typeof(SqlRepository<,>));
        return target;

        void registerRepository(Type dbContext, Type baseServiceType, Type specificServiceType, Type implementationType)
        {
            foreach (var item in getRepositoryMap(dbContext, baseServiceType, specificServiceType, implementationType))
            {
                foreach (var service in item.services)
                {
                    services.AddScoped(service, item.implementation);
                }
            }
        }
        IEnumerable<Type?> getDbSets(Type dbContext)
            => dbContext.GetProperties()
                .Where(it => it.PropertyType.IsGenericType && targetPropertyType == it.PropertyType.GetGenericTypeDefinition())
                .Select(it => it.PropertyType.GenericTypeArguments.FirstOrDefault())
                .Where(it => it is not null);
        IEnumerable<(Type[] services, Type implementation)> getRepositoryMap(Type dbContext, Type baseServiceType, Type specificServiceType, Type implementationType)
            => getDbSets(dbContext)
            .Select(dbModel =>
            {
                var dbModelContract = dbModel?.GetInterface(targetDbContractType.Name);
                if (dbModelContract is null || !dbModelContract.IsGenericType) throw new ArgumentOutOfRangeException(nameof(dbModel), $"It must implement the IDbModel<TEntity>.");
                var isKeyString = baseServiceType.GetGenericArguments().Length == 1;
                if (isKeyString && dbModelContract.GenericTypeArguments.FirstOrDefault() != typeof(string)) return (null!, null!);
                var types = isKeyString ? [dbModel] : new[] { dbModel, dbModelContract.GenericTypeArguments.FirstOrDefault() };
                var services = new Type[]
                {
                    baseServiceType.MakeGenericType(types!),
                    specificServiceType.MakeGenericType(types!),
                };
                return (services, implementation: implementationType.MakeGenericType(types!));
            }).Where(it => it.services is not null && it.implementation is not null);
    }

    /// <summary>
    /// Complete SQL setup.
    /// </summary>
    /// <param name="target">The SQL setup</param>
    public static void Build(this SqlSetup target)
        => Build(target, _ => { });

    /// <summary>
    /// Complete SQL setup.
    /// </summary>
    /// <param name="target">The SQL setup</param>
    /// <param name="config">The interceptor configuration</param>
    public static void Build(this SqlSetup target, Action<SqlInterceptorBuilder> config)
    {
        var builder = SqlInterceptorBuilder.Default;
        var store = target.ConnectionStoreBuilder.Build(builder);
        target.ServiceCollection.AddSingleton<SqlConnectionStore>(store);

        config?.Invoke(builder);
        foreach (var item in builder.InterceptorTypes)
        {
            target.ServiceCollection.AddKeyedScoped(item, store);
        }
    }
}