using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Data;
using TTSS.Infra.Data.MongoDB.Models;

namespace TTSS.Infra.Data.MongoDB;

/// <summary>
/// Helper extension methods for register MongoDB repository.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Setup MongoDB repository.
    /// </summary>
    /// <param name="target">The service collection</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Resumable MongoDB setup</returns>
    public static MongoDbSetup SetupMongoDatabase(this IServiceCollection target, string databaseName, string connectionString)
        => new(target, new(databaseName, connectionString));

    /// <summary>
    /// Setup MongoDB repository.
    /// </summary>
    /// <param name="target">The MongoDB setup</param>
    /// <param name="databaseName">The database name</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Resumable MongoDB setup</returns>
    public static MongoDbSetup SetupMongoDatabase(this MongoDbSetup target, string databaseName, string connectionString)
    {
        target.ConnectionStoreBuilder.SetupDatabase(databaseName, connectionString);
        return target;
    }

    /// <summary>
    /// Add MongoDB context for dependency injection.
    /// </summary>
    /// <typeparam name="TMongoDbContext">Type of MongoDb context</typeparam>
    /// <param name="target">The MongoDB setup</param>
    /// <returns>Resumable MongoDB setup</returns>
    /// <exception cref="ArgumentNullException">The MongoDbSetup object is required</exception>
    public static MongoDbSetup AddDbContext<TMongoDbContext>(this MongoDbSetup target)
        where TMongoDbContext : IMongoDbContext
    {
        var builder = target?.ConnectionStoreBuilder ?? throw new ArgumentNullException(nameof(target.ConnectionStoreBuilder));
        var services = target?.ServiceCollection ?? throw new ArgumentNullException(nameof(target.ServiceCollection));

        var dbContextType = typeof(TMongoDbContext);
        registerRepository(dbContextType, typeof(MongoDbRepository<>), typeof(IRepository<>), typeof(IMongoDbRepository<>), typeof(MongoDbRepository<>));
        registerRepository(dbContextType, typeof(MongoDbRepository<,>), typeof(IRepository<,>), typeof(IMongoDbRepository<,>), typeof(MongoDbRepository<,>));
        return target;

        void registerRepository(Type dbContext, Type targetPropertyType, Type baseServiceType, Type specificServiceType, Type implementationType)
        {
            foreach (var item in getRepositoryMap(dbContext, targetPropertyType, baseServiceType, specificServiceType, implementationType))
            {
                builder.RegisterCollection(item.name, item.name);
                foreach (var service in item.services)
                {
                    services.AddScoped(service, item.implementation);
                }
            }
        }
        IEnumerable<(string name, Type[] services, Type implementation)> getRepositoryMap(Type dbContextType, Type targetPropertyType, Type baseServiceType, Type specificServiceType, Type implementationType)
            => dbContextType.GetProperties()
                .Where(it => it.PropertyType.BaseType is not null
                    && it.PropertyType.BaseType.IsGenericType
                    && targetPropertyType == it.PropertyType.BaseType.GetGenericTypeDefinition())
                .Select(dbModel => dbModel.PropertyType.BaseType?.GenericTypeArguments ?? Array.Empty<Type>())
                .Where(it => it.Any())
                .Select(it =>
                {
                    var services = new Type[]
                    {
                            baseServiceType.MakeGenericType(it),
                            specificServiceType.MakeGenericType(it),
                    };
                    return (it.First().Name, services, implementationType.MakeGenericType(it));
                });
    }

    /// <summary>
    /// Complete MongoDB setup.
    /// </summary>
    /// <param name="target">The MongoDB setup</param>
    public static void Build(this MongoDbSetup target)
    {
        var store = target.ConnectionStoreBuilder.Build();
        target.ServiceCollection.AddSingleton<MongoDbConnectionStore>(store);
    }
}