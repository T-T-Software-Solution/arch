using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Infra.Data.Redis.Models;

namespace TTSS.Infra.Data.Redis;

/// <summary>
/// Helper extension methods to setup Redis database.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Setup Redis repository.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="connectionString">The Redis connection string</param>
    /// <returns>Resumable Redis setup</returns>
    public static RedisSetup SetupRedisDatabase(this IServiceCollection target, string connectionString)
        => new(target, new(connectionString));

    /// <summary>
    /// Setup Redis repository.
    /// </summary>
    /// <param name="target">The Redis setup</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Resumable Redis setup</returns>
    public static RedisSetup SetupRedisDatabase(this RedisSetup target, string connectionString)
    {
        target.ConnectionStoreBuilder.SetupDatabase(connectionString);
        return target;
    }

    /// <summary>
    /// Add Redis cache model for dependency injection.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The Redis setup</param>
    /// <param name="behavior">Entity behavior</param>
    /// <returns>Resumable Redis setup</returns>
    public static RedisSetup AddGroup<TEntity>(this RedisSetup target, RedisCacheBehavior? behavior = default)
        where TEntity : RedisCacheBase<TEntity>
    {
        var targetType = typeof(RedisCacheBase<>);
        var (builder, services, implementationType, _) = ValidateAndExtractParameters<TEntity>(target, targetType, "{TEntity} must be derieved from RedisCacheBase<TEntity>.");
        services.AddSingleton(typeof(IRedisCache<>).MakeGenericType(implementationType), implementationType);
        builder.RegisterCollection(implementationType, behavior);
        return target;
    }

    /// <summary>
    /// Add Redis repository cache model for dependency injection.
    /// </summary>
    /// <typeparam name="TRepositoryCache">The Redis repository cache</typeparam>
    /// <param name="target">The Redis setup</param>
    /// <param name="behavior">Entity behavior</param>
    /// <returns>Resumable Redis setup</returns>
    public static RedisSetup AddRepositoryCache<TRepositoryCache>(this RedisSetup target, RedisCacheBehavior? behavior = default)
        where TRepositoryCache : RedisRepositoryCache
    {
        var targetType = typeof(RedisRepositoryCache<>);
        var (builder, services, implementationType, entityTypes) = ValidateAndExtractParameters<TRepositoryCache>(target, targetType, "{TRepositoryCache} must be derieved from RedisRepositoryCacheBase<TEntity>.");
        var generalType = typeof(IRepositoryCache<>).MakeGenericType(entityTypes);
        var specificType = typeof(IRedisRepositoryCache<>).MakeGenericType(entityTypes);
        services.AddSingleton(generalType, implementationType);
        services.AddSingleton(specificType, pvd => pvd.GetService(generalType)!);
        builder.RegisterCollection(implementationType, behavior);
        return target;
    }

    /// <summary>
    /// Complete Redis setup.
    /// </summary>
    /// <param name="target">The Redis setup</param>
    public static void Build(this RedisSetup target)
    {
        var store = target.ConnectionStoreBuilder.Build();
        target.ServiceCollection.AddSingleton<RedisConnectionStore>(store);
    }

    private static (RedisConnectionStoreBuilder builder, IServiceCollection services, Type implementationType, Type[] entityTypes)
        ValidateAndExtractParameters<TParameter>(RedisSetup setup, Type targetType, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(setup);
        var builder = setup.ConnectionStoreBuilder ?? throw new ArgumentNullException(nameof(setup), $"The {nameof(setup.ConnectionStoreBuilder)} must not be null.");
        var services = setup.ServiceCollection ?? throw new ArgumentNullException(nameof(setup), $"The {nameof(setup.ServiceCollection)} must not be null.");

        var implementationType = typeof(TParameter);

        var isRedisRepositoryCache = implementationType.BaseType == typeof(RedisRepositoryCache);
        if (isRedisRepositoryCache)
        {
            var entityTypes = implementationType.GetGenericArguments();
            return (builder, services, implementationType, entityTypes);
        }
        else
        {
            var isParameterValid = (implementationType.BaseType?.IsGenericType ?? false) && targetType == implementationType.BaseType.GetGenericTypeDefinition();
            if (!isParameterValid) throw new ArgumentOutOfRangeException(errorMessage, implementationType.Name);
            var entityTypes = implementationType.BaseType!.GetGenericArguments();
            return (builder, services, implementationType, entityTypes);
        }
    }
}