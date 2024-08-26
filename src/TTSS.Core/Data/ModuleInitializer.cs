using Microsoft.Extensions.DependencyInjection;
using TTSS.Core.Services;

namespace TTSS.Core.Data;

/// <summary>
/// Helper extension methods for register in-memory repository.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Registers in-memory repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    /// <param name="target">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterInMemoryRepository<TEntity, TKey>(this IServiceCollection target)
        where TEntity : class, IDbModel<TKey>
        where TKey : notnull
        => target.AddSingleton<IRepository<TEntity, TKey>>(pvd => new InMemoryRepository<TEntity, TKey>(pvd.GetRequiredService<IMappingStrategy>(), it => it.Id));

    /// <summary>
    /// Registers in-memory repository.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <param name="target">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection RegisterInMemoryRepository<TEntity>(this IServiceCollection target)
        where TEntity : class, IDbModel<string>
        => target.AddSingleton<IRepository<TEntity>>(pvd => new InMemoryRepository<TEntity>(pvd.GetRequiredService<IMappingStrategy>(), it => it.Id));
}